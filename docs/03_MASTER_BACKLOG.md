# Master Backlog

## Epic 1: Initial Discord Communication
- [x] Sub-task 1.1: Configure Discord settings and application settings abstraction (`IAppSetting`, `DiscordSettings`, server filtering via `AllowServerIds`).
- [x] Sub-task 1.2: Implement `DiscordConnectionService` (`ICommunicationConnectionService`) and `DiscordCommunicationContextService` (`ICommunicationContextService`) with Discord.Net.
- [x] Sub-task 1.3: Implement `DiscordBotBackgroundService` in API to initialize and maintain the Discord WebSocket gateway connection.
- [x] Sub-task 1.4: Support normal messages with bot mention filtering, immediate "Thinking..." acknowledgment, and `MessageBusinessLogic.ProcessNormalMessageAsync` responding "Received your message.".
- [x] Sub-task 1.5: Support slash command `/ping` via `DiscordSlashCommand` in API, invoking `MessageBusinessLogic.ProcessSlashPingAsync` responding "Pong".
