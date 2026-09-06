# Product Requirements Document (PRD)

## 1. Project Overview
**Project Name:** League of Legends AI Coach (Discord Bot)
**Description:** A Discord Bot that serves as a personal League of Legends coach. The bot leverages the Model Context Protocol (MCP) to aggregate and analyze data from the Riot API, OP.GG, and YouTube, providing in-depth analytics on player performance, match breakdowns, and practical matchup coaching directly through the Discord interface.

## 2. Target Audience
- League of Legends players looking to improve their gameplay.
- Discord server communities focused on gaming and eSports.
- Coaches and analysts needing quick insights and aggregated data.

## 3. Core Features
- **Player Profiling:** Processes queries using the `Name#Tag` format. Analyzes the player's champion pool, laning fundamentals, strengths, weaknesses, and identifies specific areas for improvement.
- **Match Analysis:** Provides detailed post-match breakdowns. Analyzes resource differentials (gold/XP) over time, extracts key events, and evaluates map control, rotation efficiency, and laning phase performance.
- **Matchup Coaching:** Integrates OP.GG statistics and YouTube matchup guidance (e.g., Champion A vs. Champion B). Searches for gameplay guide videos, parses transcripts, and summarizes actionable laning tips.
- **Drafting Support:** Quickly retrieves lists of counter champions and optimal team synergies.
- **Persistent Conversational Memory:** The bot remembers previous context in the same channel, allowing multiple users in a server to discuss and follow up on the same coaching session.
- **Dynamic Configuration:** Supports runtime configuration adjustments for Admins and per-server (guild) customized settings.

## 4. Key Constraints & Non-Functional Requirements
- **Discord API Timeout Limit:** Discord strictly requires bots to acknowledge slash commands within 3 seconds. The bot must immediately invoke a `defer reply` state upon receiving the command, route the processing pipeline to the background, and return the final analysis via an edit reply or follow-up message.
- **Riot API Rate Limiting & Persistence:** Fetching granular match histories and timeline data will rapidly exhaust standard Development API Key rate limits. The application must persist profiles and timelines in SQLite with an expiration column, only re-fetching when expired or explicitly forced by the user.
- **Anti-Spam & Concurrency Lock:** The bot must track active processing states per channel. If a user sends new requests while the bot is already processing a command in that channel, it should reject the request with a polite "Bot is on process please wait a little bit" message to prevent spam and race conditions. It should also trigger the Discord "typing" indicator to signal users that it is working.

## 5. User Flows
1. **Command Execution:** User types a slash command (e.g., `/coach profile Name#Tag`).
2. **Immediate Acknowledgement:** Bot defers the interaction to prevent the 3-second timeout.
3. **Context Retrieval:** Bot retrieves channel conversation history and configurations from SQLite / IMemoryCache.
4. **Data Aggregation:** Bot makes concurrent/sequential requests to MCP servers (Riot API, OP.GG, YouTube) via HTTP/SSE. It checks the local SQLite database first to avoid redundant API calls.
5. **LLM Processing:** Aggregated data and previous channel context are sent to an LLM to generate insights.
6. **Final Response:** The bot updates the initial deferred message with a formatted embed containing the coaching analysis, and saves the new interaction to the SQLite conversation history (pruning old messages based on `MaxChatPerSession`).
