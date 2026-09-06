# 🤖 AI CONTEXT & NAVIGATION INDEX

> **@AI-AGENT DIRECTIVE (CRITICAL):** 
> You are AI Agent (or any acting AI Assistant). BEFORE generating any code, modifying files, or planning a task, you MUST parse this file and adhere strictly to the workflow and constraints defined below. 
> DO NOT rely on your general knowledge if it conflicts with the documentation in the `/docs` folder.

## 1. CONTEXT MAP
All project requirements, architecture, and task breakdowns are located in the `/docs` directory. Navigate them in this order:

- 📄 `docs/01_PRD.md`: Core project goals, target audience, and business rules.
- 📄 `docs/02_ARCHITECTURE.md`: Tech stack (e.g., C#, ASP.NET Core), coding standards, folder structure **(MUST READ BEFORE CODING)**
- 📄 `docs/03_MASTER_BACKLOG.md`: High-level feature lists and release milestones.
- 📁 `docs/features/`: Contains specific feature requirements. Each file here contains a specific Epic and its detailed Sub-task Checklists.

## 2. AGENT WORKFLOW
When the user assigns you a feature or a bug, you MUST execute the following loop:

1. **Acknowledge & Locate:** Identify which feature file in `docs/features/` matches the request.
2. **Context Gathering:** Read the relevant `docs/features/<feature_name>.md` AND `docs/02_ARCHITECTURE.md`. 
3. **Plan & Breakdown:** If the feature is not broken down into sub-tasks yet, generate a checklist of specific sub-tasks in the feature file and ask the user for approval.
4. **Execution:** Write the code strictly following the Architecture standards.
5. **Update State:** Once a sub-task is completed and verified, mark it as `[x]` in the corresponding feature file checklist.

## 3. GLOBAL CONSTRAINTS
- **NO HALLUCINATION OF DEPENDENCIES:** Do not introduce new libraries or packages without asking for user approval first. Stick to the tech stack defined in `02_ARCHITECTURE.md`.
- **NO DESTRUCTIVE OVERWRITES:** If modifying existing working code, explain the refactor logic briefly before applying changes.
- **STATE MANAGEMENT:** Always maintain clean state patterns (e.g., Redux for React front-end, Entity Framework Core for backend) as defined in the architecture.
- **CONCISENESS:** When communicating with the user, be concise, direct, and objective. Point out flaws in the user's logic if their request contradicts the architecture.

## 4. CURRENT PROJECT STATUS
- **Phase:** Initialization / Architecture Setup
- **Active Task:** Setting up workspace and initial context templates.