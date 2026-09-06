# System Architecture

## 1. High-Level Architecture
The system is self-hosted, consisting of a core C# .NET application and several Model Context Protocol (MCP) servers.

### Components
- **Core Application (`DiscordLoLCoach`)**: A C# ASP.NET Core worker service. Handles Discord interactions, orchestrates data gathering, manages layered caching (IMemoryCache + SQLite), and processes business logic with LLM integration.
- **MCP Servers (Docker Containers)**:
  - `MrBridgeHQ/mcp-server-lol`: Custom container interacting directly with the Riot API.
  - `opgginc/opgg-mcp`: Scrapes and provides OP.GG statistical data.
  - `youtube-mcp`: Searches for YouTube videos and extracts transcripts.

## 2. Technology Stack
- **Language/Framework:** C# 12 / .NET 8 (or latest) Worker Service / ASP.NET Core
- **Discord Library:** Discord.Net
- **Database (Persistence):** SQLite (using Entity Framework Core or Dapper)
- **Caching Layer:** .NET `IMemoryCache` 
- **MCP Communication:** Standard HTTP/SSE to communicate with MCP servers.

## 3. Data Storage & Caching Strategy
- **Profiles & Timelines:** Stored in SQLite for persistence. Records include an expiration (`exp`) column to determine when re-fetching is required. Users can also manually prompt to force a re-fetch.
- **Conversations:** Saved in SQLite using the **Channel ID** as the session key. This allows multiple users in the same channel to participate in a shared context with the bot. `IMemoryCache` acts as a fast-read layer; if a cache miss occurs, the history is loaded from SQLite.
  - **Pruning:** When retrieving or saving chats, the system ensures the number of saved messages does not exceed the `MaxChatPerSession` limit by deleting older entries from SQLite to prevent database bloat.
  - **Processing Lock:** `IMemoryCache` stores a boolean lock for the Channel ID while a request is actively being processed to prevent concurrent overlapping executions and spam.
- **Configuration Hierarchy (3 Levels):** 
  - *Root Level:* `appsettings.json` (e.g., `MaxChatPerSession`).
  - *Admin Level:* Saved in SQLite. Runtime-configurable global settings applied across all conversations.
  - *Bot (Server/Guild) Level:* Saved in SQLite. Settings specific to a Discord Server ID (e.g., long-term memory toggles, specific behavioral prompts).
  - *Settings Caching:* All configuration settings are cached in `IMemoryCache` for fast access.

## 4. Communication Flow
1. **Discord -> Core App:** Users trigger slash commands or mention the bot in a channel.
2. **Spam Check & Lock:** Core App checks if a processing lock exists for the channel. If yes, it immediately returns a "Bot is on process please wait a little bit" message. Otherwise, it sets a lock and triggers the channel "typing" state.
3. **Config & Context Retrieval:** 
   - Core App fetches hierarchical configuration (Root -> Admin -> Server) via IMemoryCache/SQLite.
   - Core App fetches channel conversation history via IMemoryCache/SQLite.
4. **Data Fetching (Profiles/Timelines):**
   - Checks SQLite for valid, non-expired profile/timeline data.
   - If expired or not present, fetches from MCP servers via HTTP/SSE and saves to SQLite.
5. **Core App -> LLM:** The aggregated text/JSON data, config settings, and channel chat history are passed to the LLM (e.g., Gemini) for natural language coaching generation.
6. **Response & Persistence:** 
   - Processed response sent back to the Discord channel via `ModifyOriginalResponseAsync` or `FollowupAsync`.
   - New User + Bot messages appended to SQLite (and IMemoryCache), pruning old messages if exceeding `MaxChatPerSession`.
   - Channel processing lock is released.

## 5. Folder Structure (Proposed)
```text
DiscordLoLCoach/
├── Bot/                 # Discord.Net module setup, command handlers, and Discord client lifecycle
├── Services/            # Business logic, caching logic, LLM integration, Config Managers
├── McpClients/          # HTTP/SSE clients to interface with the various MCP servers
├── Data/                # SQLite DbContext, Migrations, Repositories
├── Models/              # Data structures and domain models
├── Program.cs           # Application entry point and DI configuration
└── docs/                # Documentation and Context Maps
```

## 6. Design Patterns & Standards
- **Dependency Injection (DI):** Strictly use the built-in Microsoft.Extensions.DependencyInjection for all services, clients, and modules.
- **Asynchronous Programming:** Use `async/await` heavily. Avoid `.Result` or `.Wait()`.
- **Error Handling:** Graceful degradation. If YouTube transcription fails, the bot should still return Riot API/OP.GG stats with a note about missing video insights.
