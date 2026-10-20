# Epic 1: Initial Discord Communication

## Feature Overview
Establish initial Discord communication for the League of Legends AI Coach bot using `Discord.Net`. This includes application configuration management, background bot lifecycle service, bot mention detection for normal messages with immediate "Thinking..." acknowledgment, and the `/ping` slash command.

## Architecture Alignment
- **Application Layer**: Contains `IAppSetting`, `DiscordSettings`, `ICommunicationConnectionService`, `ICommunicationContextService`, and `MessageBusinessLogic`.
- **API Layer**: Implements the Discord presentation and delivery mechanism, including `DiscordConnectionService` (`ICommunicationConnectionService`), `DiscordCommunicationContextService` (`ICommunicationContextService`), `DiscordEventHandler` (`IDiscordEventHandler`), `DiscordBotBackgroundService` (`BackgroundService`), and `DiscordSlashCommand` (`InteractionModuleBase<SocketInteractionContext>`).

## Sub-task Checklist
- [x] **Sub-task 1.1**: Initial Discord Settings
  - Defined `DiscordSettings` (`Token`, `AllowServerIds`).
  - Defined `IAppSetting` and implemented `AppSetting` singleton in Application layer.
  - Configured `appsettings.json` with `DiscordSettings`.
  - Added `IsServerAllowed(ulong? serverId)` validation logic (null, empty, or whitespace entries allow all servers).
- [x] **Sub-task 1.2**: Communication Services Implementation
  - Installed `Discord.Net` in Infrastructure and API projects.
  - Implemented `DiscordCommunicationContextService : ICommunicationContextService` with message editing/followup handling.
  - Implemented `DiscordConnectionService : ICommunicationConnectionService` to manage `DiscordSocketClient` & `InteractionService`.
- [x] **Sub-task 1.3**: Background Service Integration
  - Implemented `DiscordBotBackgroundService` in API layer inheriting `BackgroundService`.
  - Triggered `ICommunicationConnectionService.StartAsync()` inside background service.
- [x] **Sub-task 1.4**: Normal Message Processing
  - Check if incoming message is not from a bot and belongs to an allowed server.
  - Check if the message mentions the current AI bot.
  - Send initial acknowledgment "Thinking...".
  - Invoke `MessageBusinessLogic.ProcessNormalMessageAsync()` replying "Received your message.".
- [x] **Sub-task 1.5**: Slash Command `/ping`
  - Implemented `DiscordSlashCommand` class in API layer with method `PingAsync()` decorated with `[SlashCommand("ping", "Replies with a pong!")]`.
  - Invoked `MessageBusinessLogic.ProcessSlashPingAsync()` replying "Pong".
