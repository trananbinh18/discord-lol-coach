# League of Legends AI Coach (Discord Bot)

A Discord Bot that serves as a personal League of Legends coach. The bot leverages the Model Context Protocol (MCP) to aggregate and analyze data from the Riot API, OP.GG, and YouTube, providing in-depth analytics on player performance, match breakdowns, and practical matchup coaching directly through the Discord interface.

## 🌟 Core Features

- **Player Profiling:** Processes queries using the `Name#Tag` format. Analyzes the player's champion pool, laning fundamentals, strengths, weaknesses, and identifies specific areas for improvement.
- **Match Analysis:** Provides detailed post-match breakdowns. Analyzes resource differentials (gold/XP) over time, extracts key events, and evaluates map control, rotation efficiency, and laning phase performance.
- **Matchup Coaching:** Integrates OP.GG statistics and YouTube matchup guidance (e.g., Champion A vs. Champion B). Searches for gameplay guide videos, parses transcripts, and summarizes actionable laning tips.
- **Drafting Support:** Quickly retrieves lists of counter champions and optimal team synergies.
- **Persistent Conversational Memory:** The bot remembers previous context in the same channel, allowing multiple users in a server to discuss and follow up on the same coaching session.
- **Dynamic Configuration:** Supports runtime configuration adjustments for Admins and per-server (guild) customized settings.

## 🤖 AI Agent Directive

> **CRITICAL:** Any AI Agent (or acting AI Assistant) interacting with this repository **MUST** read and parse [`DiscordLoLCoach/AI_CONTEXT_INDEX.md`](DiscordLoLCoach/AI_CONTEXT_INDEX.md) before generating code, modifying files, or planning a task.

All project requirements, architecture details, and task breakdowns are located in the `DiscordLoLCoach/docs/` directory.

- [`docs/01_PRD.md`](DiscordLoLCoach/docs/01_PRD.md): Core project goals, target audience, and business rules.
- `docs/02_ARCHITECTURE.md`: Tech stack, coding standards, and folder structure.
- `docs/03_MASTER_BACKLOG.md`: High-level feature lists and release milestones.

## 🏗️ Technical Constraints & Architecture

- **Discord API Timeout Limit:** Immediately defers replies to avoid the 3-second timeout and processes requests in the background.
- **Riot API Rate Limiting & Persistence:** Caches profiles and timelines in SQLite to avoid exhausting Riot API rate limits, only re-fetching when expired or explicitly forced by the user.
- **Anti-Spam & Concurrency Lock:** Rejects new requests with a polite message if a command is already being processed in that channel. Uses Discord's "typing" indicator to signal users that it is working.

---

**Project Status:** Initialization / Architecture Setup
