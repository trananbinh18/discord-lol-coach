# System Architecture

## 1. High-Level Architecture
The system follows a Clean Architecture (Onion Architecture) design, consisting of a core C# .NET application and several Model Context Protocol (MCP) servers. The architecture is designed to decouple business logic from infrastructure concerns like databases, external APIs, and the Discord platform.

### Components
- **Core Application (`DiscordLoLCoach`)**: A C# application structured into several layers (Domain, Application, Infrastructure, Presentation/API). It handles Discord interactions, orchestrates data gathering via MCP, and processes business logic with LLM integration using `Microsoft.Extensions.AI`.
- **MCP Servers (Docker Containers)**:
  - `MrBridgeHQ/mcp-server-lol`: Custom container interacting directly with the Riot API.
  - `opgginc/opgg-mcp`: Scrapes and provides OP.GG statistical data.
  - `youtube-mcp`: Searches for YouTube videos and extracts transcripts.

## 2. Project Structure (Clean Architecture)

The solution is divided into the following layers:

### 1. Domain Layer (`DiscordLoLCoach.Domain`)
- **Type**: Class Library (`Microsoft.NET.Sdk`)
- **Responsibility**: Contains the core business entities, enums, and domain logic. This layer represents the data structures for persistence (e.g., Profiles, Timelines, Conversations).
- **Dependencies**: None. It is the innermost layer and does not depend on any other project or external infrastructure frameworks (like EF Core).

### 2. Application Layer (`DiscordLoLCoach.Application`)
- **Type**: Class Library (`Microsoft.NET.Sdk`)
- **Responsibility**: Contains the business logic handlers and defines the interfaces (contracts) for the application. It orchestrates the flow of data but relies on abstractions for any external operations.
- **Key Interfaces Defined Here**:
  - `IApplicationDbContext`: Abstraction for database operations.
  - `ICacheMemory`: Abstraction for temporary memory (e.g., storing conversational context, locks).
  - `IAgentService`: Abstraction for LLM interactions (using `Microsoft.Extensions.AI`).
  - `IMCPOrchestrator`: Orchestrates fetching available MCP tools and routing execution.
  - `ICommunicationService`: Abstraction for sending messages back to the user (e.g., via Discord).

### 3. Infrastructure Layer (`DiscordLoLCoach.Infrastructure`)
- **Type**: Class Library (`Microsoft.NET.Sdk`)
- **Responsibility**: Contains the implementations of the interfaces defined in the Application layer. This is where the code interacts with the outside world.
- **Implementations Include**:
  - SQLite DbContext implementation (`IApplicationDbContext`).
  - LocalInMemoryCache implementation (`ICacheMemory`).
  - Discord.NET implementation (`ICommunicationService`). Used for asynchronous interactions like sending delayed follow-up responses after long tasks.
  - MCP Client implementations.

### 4. Presentation / API Layer (`DiscordLoLCoach.API`)
- **Type**: Web API (`Microsoft.NET.Sdk.Web`)
- **Responsibility**: The entry point of the application. It configures Dependency Injection, sets up middleware, and exposes HTTP endpoints. Crucially, it receives incoming Discord Interaction webhooks, immediately acknowledges them (e.g., "thinking" state), and delegates the heavy lifting to the Application layer.

## 3. Technology Stack
- **Language/Framework:** C# / .NET 9 Worker Service / ASP.NET Core
- **Discord Library:** Discord.Net
- **Database (Persistence):** SQLite (using Entity Framework Core)
- **AI Integration:** `Microsoft.Extensions.AI` (supports swapping LLM providers like Gemini, Claude, OpenAI)
- **MCP Communication:** Standard HTTP/SSE to communicate with MCP servers.

## 4. Communication Flow
1. **Event Trigger (Webhook):** A user interacts with the bot on Discord (e.g., via a slash command). Discord sends an HTTP request to the API layer.
2. **API Layer (Initial Ack):** The API layer immediately acknowledges the request to Discord (putting the bot in a "thinking" state) and forwards a command to the Application layer to process the request asynchronously.
3. **Application Logic:** The Application layer handler processes the command. It uses `ICacheMemory` for state/locks and retrieves context.
4. **Tool & Context Prep:** The Application layer calls `IMCPOrchestrator` to get available tools and prepares the conversation history.
5. **LLM Interaction:** The Application layer passes tools and context to `IAgentService`.
6. **Tool Execution (if needed):** If the LLM requests a tool (e.g., Riot API stats), the `IMCPOrchestrator` executes it and returns the result to the LLM.
7. **Response Generation:** The LLM generates the final coaching response.
8. **Output & Persistence:** The Application layer uses `ICommunicationService` to send the response to Discord and `IApplicationDbContext` to log the interaction.

## 5. Design Patterns & Standards
- **Dependency Injection (DI):** Strictly use the built-in Microsoft.Extensions.DependencyInjection.
- **CQRS Pattern (Optional but recommended):** Use MediatR in the Application layer to separate read (Query) and write (Command) operations for better organization of logic handlers.
- **Asynchronous Programming:** Use `async/await` heavily.
