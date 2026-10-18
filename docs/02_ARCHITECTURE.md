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

### 1. Domain / Data Layer (`DiscordLoLCoach.Data`)
- **Type**: Class Library (`Microsoft.NET.Sdk`)
- **Responsibility**: Contains the core business entities, enums, and domain logic. This layer represents the data structures for persistence (e.g., Profiles, Timelines, Conversations).
- **Dependencies**: None. It is the innermost layer and does not depend on external infrastructure frameworks.

### 2. Application Layer (`DiscordLoLCoach.Application`)
- **Type**: Class Library (`Microsoft.NET.Sdk`)
- **Responsibility**: Contains the business logic handlers and defines the interfaces (contracts) for the application. It orchestrates the flow of data but relies on abstractions for any external operations.
- **Key Folders**:
  - `Business/`: Contains the main business logic of the application (e.g., `MessageBusinessLogic`, command orchestration, and coaching flow handlers).
  - `Interfaces/`: Abstractions and contracts for domain, communication, and infrastructure services.
  - `Extensions/`: Service collection extensions for registering Application layer dependencies.
- **Key Interfaces Defined Here**:
  - `ICommunicationConnectionService`: Manages and maintains the persistent connection (e.g., WebSocket) to communication services like Discord. Registered as a long-lived Singleton service.
  - `ICommunicationContextService`: Scoped service tied to a specific incoming user message. Stores interaction context:
    - `UserMessage`: Content or payload of the user's message.
    - `UserMessageId`: Unique identifier of the incoming user message.
    - `ChannelId`: Identifier of the channel where the message was received.
    - `AckMessage`: Initial acknowledgment message content/object (e.g., "thinking" state or deferred reply).
    - `AckMessageId`: Identifier of the acknowledgment message.
  - `IApplicationDbContext`: Abstraction for database operations.
  - `ICacheMemory`: Abstraction for temporary memory (e.g., storing conversational context, locks).
  - `IAgentService`: Abstraction for LLM interactions (using `Microsoft.Extensions.AI`).
  - `IMCPOrchestrator`: Orchestrates fetching available MCP tools and routing execution.

### 3. Infrastructure Layer (`DiscordLoLCoach.Infrastructure`)
- **Type**: Class Library (`Microsoft.NET.Sdk`)
- **Responsibility**: Contains the implementations of the interfaces defined in the Application layer. This is where the code interacts with the outside world.
- **Implementations Include**:
  - Discord.NET implementation of `ICommunicationConnectionService` (manages persistent WebSocket connection to Discord Gateway).
  - Discord.NET implementation of `ICommunicationContextService` (scoped service managing message metadata and replying/editing messages in Discord channels).
  - SQLite DbContext implementation (`IApplicationDbContext`).
  - LocalInMemoryCache implementation (`ICacheMemory`).
  - MCP Client implementations (`IMCPOrchestrator`).

### 4. Presentation / API Layer (`DiscordLoLCoach.API`)
- **Type**: Web API (`Microsoft.NET.Sdk.Web`)
- **Responsibility**: The entry point of the application. It configures Dependency Injection, sets up middleware, initializes long-lived services (e.g., `ICommunicationConnectionService` maintaining the Discord WebSocket connection), and exposes HTTP endpoints (health checks, monitoring).

## 3. Technology Stack
- **Language/Framework:** C# / .NET 9 Worker Service / ASP.NET Core
- **Discord Library:** Discord.Net (WebSocket Gateway connection via `DiscordSocketClient`)
- **Database (Persistence):** SQLite (using Entity Framework Core)
- **AI Integration:** `Microsoft.Extensions.AI` (supports swapping LLM providers like Gemini, Claude, OpenAI)
- **MCP Communication:** Standard HTTP/SSE to communicate with MCP servers.

## 4. Communication Flow
1. **Connection Maintenance:** `ICommunicationConnectionService` establishes and maintains a persistent WebSocket connection to the Discord Gateway.
2. **Event Trigger:** A user sends a message or invokes a command on Discord. Discord pushes the event across the WebSocket connection.
3. **Scope & Context Initialization:** Upon event arrival, a new Dependency Injection scope is created. `ICommunicationContextService` is instantiated and populated with `UserMessage`, `UserMessageId`, and `ChannelId`.
4. **Immediate Acknowledgement:** An initial acknowledgment is sent back to Discord (e.g., typing indicator, thinking message, or deferred reply) to prevent timeouts. The acknowledgment details (`AckMessage`, `AckMessageId`) are stored in `ICommunicationContextService`.
5. **Business Logic Execution:** The scoped business handler in `Business/` (e.g., `MessageBusinessLogic`) takes over execution. It checks `ICacheMemory` for anti-spam locks and retrieves channel context.
6. **Tool & Context Prep:** The business logic calls `IMCPOrchestrator` to gather available MCP tools and prepares the conversation history.
7. **LLM Interaction:** The business logic passes tools and context to `IAgentService`.
8. **Tool Execution (if needed):** If the LLM requests a tool (e.g., Riot API stats, OP.GG meta), `IMCPOrchestrator` executes it and returns the result to the LLM.
9. **Response Generation:** The LLM generates the final coaching response.
10. **Output & Persistence:** The business logic updates the initial acknowledgment or sends the final response via `ICommunicationContextService` (using `AckMessageId` / `ChannelId`), and logs the interaction using `IApplicationDbContext`.

## 5. Design Patterns & Standards
- **Dependency Injection (DI) & Lifetimes:** Strictly use built-in Microsoft.Extensions.DependencyInjection:
  - `ICommunicationConnectionService`: **Singleton** (long-lived service that maintains the WebSocket connection).
  - `ICommunicationContextService`: **Scoped** (isolated per incoming user message/interaction).
  - Business Handlers (`MessageBusinessLogic`): **Scoped** or **Transient**.
- **Asynchronous Programming & Naming Conventions:**
  - Use `async/await` heavily for all I/O-bound operations.
  - Every asynchronous method must strictly follow the `Async` suffix naming convention (e.g., `ProcessMessageAsync`, `SendAckAsync`, `GetProfileAsync`).
