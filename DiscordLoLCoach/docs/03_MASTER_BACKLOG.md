# Master Backlog

## Epic 1: Project Initialization & Infrastructure
- [ ] Sub-task 1.1: Initialize C# .NET Worker project (`DiscordLoLCoach`).
- [ ] Sub-task 1.2: Set up Discord.Net framework, bot token configuration, and slash command registration structure.
- [ ] Sub-task 1.3: Set up Entity Framework Core with SQLite provider and create initial migrations.
- [ ] Sub-task 1.4: Implement a basic "ping" slash command with a deferred reply to test the 3-second timeout bypass.

## Epic 2: Data Persistence & Configuration
- [ ] Sub-task 2.1: Create SQLite models and repositories for Data (Profiles, Timelines), Conversations (by Channel ID), and Configurations (Admin, Server).
- [ ] Sub-task 2.2: Implement `IMemoryCache` wrapper service as a first-layer cache for SQLite data.
- [ ] Sub-task 2.3: Implement Hierarchical Configuration Provider (merging Root `appsettings.json`, Admin SQLite, and Server SQLite settings).
- [ ] Sub-task 2.4: Implement Conversation history logic with `MaxChatPerSession` pruning during read/write operations.
- [ ] Sub-task 2.5: Implement Channel-level processing locks (anti-spam) using `IMemoryCache` and Discord typing state indicator.

## Epic 3: MCP Integrations
- [ ] Sub-task 3.1: Build base HTTP/SSE client architecture for communicating with standard MCP servers.
- [ ] Sub-task 3.2: Integrate `MrBridgeHQ/mcp-server-lol` for Riot API data (Summoner Profile, Match History, Match Timelines), with SQLite persistence and expiration (`exp`) checks.
- [ ] Sub-task 3.3: Integrate `opgginc/opgg-mcp` for meta statistics and champion matchups.
- [ ] Sub-task 3.4: Integrate `youtube-mcp` for searching guide videos and fetching transcripts.

## Epic 4: Core Coaching Features
- [ ] Sub-task 4.1: Implement LLM integration service (e.g., using Gemini API) to process aggregated data and channel context.
- [ ] Sub-task 4.2: Build `/coach profile <Name#Tag>` feature (Champion pool, strengths, weaknesses) with manual force re-fetch parameter.
- [ ] Sub-task 4.3: Build `/coach match <MatchID>` feature (Post-match breakdown, resource differentials).
- [ ] Sub-task 4.4: Build `/coach matchup <MyChamp> <EnemyChamp>` feature (Video guides, laning tips, OP.GG stats).
- [ ] Sub-task 4.5: Build `/coach draft` feature for counter lists and team synergies.

## Epic 5: Refinement & Testing
- [ ] Sub-task 5.1: Error handling polish (graceful failures when APIs rate limit or MCP servers are down).
- [ ] Sub-task 5.2: UI/UX polish using Discord rich embeds, buttons, and formatting.
- [ ] Sub-task 5.3: End-to-end testing in a private Discord server.
