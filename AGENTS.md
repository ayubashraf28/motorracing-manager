# AGENTS.md

This file defines how AI agents should work in this repository.
It applies to the entire repo unless a deeper `AGENTS.md` exists in a subdirectory.

## 1. Mission

Build and maintain `motorracing-manager` as a layered Unity codebase where:

- business logic is engine-free and testable,
- Unity-specific code is thin adapter/presentation code,
- architecture and process decisions remain documented and traceable.

## 1.1 Owner Directive (Non-Negotiable)

The repository owner's standing instruction is:

- Treat the AI as the development team responsible for implementation.
- When given a plan, understand it fully and implement it cleanly and professionally.
- Ask clarifying questions whenever there is doubt or ambiguity.
- Do not cut corners; complete work to a high standard end-to-end.

## 2. Read Order Before Making Changes

Read these files first, in this order:

1. `Documentation/README.md`
2. `Documentation/Product/00-project-overview.md`
3. `Documentation/Product/01-game-pillars.md` (non-negotiable design rules)
4. `Documentation/Architecture/00-system-overview.md`
5. `Documentation/Architecture/01-layering-and-rules.md`
6. `Documentation/Development/01-workflow-and-doc-updates.md`
7. `Documentation/Development/02-coding-standards.md`
8. `Documentation/Operations/00-ci-cd.md`
9. Latest ADRs in `Documentation/ADR/`

Then inspect code in `UnityProject/Packages/` before proposing or implementing changes.

## 3. Source of Truth Priority

If guidance conflicts, use this precedence:

1. Current repository code/configuration (what actually runs)
2. Accepted ADRs (`Documentation/ADR/`)
3. Architecture and operations docs
4. Root `README.md`
5. This file

If docs are stale relative to code, update docs in the same branch/PR.

## 4. Repository Map

- `CLAUDE.md`: Tech Architect reference (design principles, architecture, phase map, guardrails)
- `AGENTS.md`: this file — dev team rules and process
- `UnityProject/Packages/`: all local package code
- `UnityProject/Assets/_Project/`: project-owned scenes/resources/settings
- `UnityProject/ProjectSettings/`: Unity project configuration
- `.github/workflows/unity-ci.yml`: CI workflow
- `Documentation/`: project documentation system

## 5. Design Principles (Non-Negotiable)

Every implementation decision must align with these six principles:

| Principle | What it means for you |
|---|---|
| **Deterministic** | All RNG via seeded `System.Random` — never `UnityEngine.Random`. Use `int`/`long` for lap times (ms), money (cents), ratings (0–10000 basis points). No `float`/`double` in simulation hot paths. Same seed + same decisions = identical outcomes on every platform. |
| **Data-Driven** | Every tuning/balance value lives in an external data file (JSON or ScriptableObject definition). Zero magic numbers in code. If you write a literal number in a calculation, it must come from a loaded definition instead. |
| **Definition/Runtime Split** | Immutable definitions (track layouts, series rules, tyre compound specs) are separate types from mutable runtime state (part wear, contract status, finances). Definitions are loaded once and never mutated. Runtime state is what gets saved. |
| **Layered Simulation** | Lap time is built from an ordered list of ranked modifiers (base pace, driver skill, tyre wear, fuel load, weather, etc.) — not raw stat addition. Every modifier must be individually inspectable and testable. |
| **Testable Domain** | Domain logic has zero engine/UI dependencies. Pure functions, serializable state. If it can't run in an EditMode test without a scene, it's in the wrong layer. |
| **Mod-Ready** | Swapping a complete definition pack (different tracks, teams, rules) must produce a valid, playable game with no code changes. |

## 6. Architecture Rules (Non-Negotiable)

Dependency direction:

`Core -> Domain -> Sim -> App -> Persistence -> Unity -> UI`

Required constraints:

1. `Core`, `Domain`, `Sim`, `App`, `Persistence` must remain engine-free (`noEngineReferences: true`).
2. `UnityEngine` usage is allowed only in `Unity` and `UI` packages.
3. `MonoBehaviour` classes must not hold business rules.
4. Composition root starts from `MotorracingManager.Unity.GameBootstrap`.
5. Persist domain state, not transient scene object state.
6. No `static` mutable singletons — use constructor injection via composition root.
7. All public APIs must be interface-first (enables mocking and swappable implementations).

When adding dependencies, update asmdefs and verify dependency direction remains valid.

## 7. Package Ownership Index (Where to Implement)

- Shared primitives/value types: `com.alivastudio.motorracingmanager.core`
- Domain entities/contracts: `com.alivastudio.motorracingmanager.domain`
- Deterministic simulation: `com.alivastudio.motorracingmanager.sim`
- Use cases/orchestration: `com.alivastudio.motorracingmanager.app`
- Save/load and migrations: `com.alivastudio.motorracingmanager.persistence`
- Unity adapters/bootstrap: `com.alivastudio.motorracingmanager.unity`
- UI flow/controllers: `com.alivastudio.motorracingmanager.ui`

## 8. Process Rules

Branch and merge policy:

- Do not work directly on `main`.
- Use `feature/*` branches.
- Merge through pull requests only.

Current branch protection baseline:

- `main` is protected and PR-only.
- Force-push and branch deletion on `main` are disabled.

Temporary CI policy note (current state):

- Required status check `test-and-build-windows` is temporarily optional while Unity activation is remediated.
- Track activation remediation in issue `#2`.
- Re-enable required check after activation is validated.

## 8.1 Phase Map & Plan Notation

Work is organized into phases. Plans use tags to indicate scope gate and dependencies:

- **[MVP]** = Required for minimum viable prototype (Phases 0–3 + minimal AI + wireframe UI)
- **[ALPHA]** = Required for feature-complete alpha (all phases, single series multi-season)
- **[BETA]** = Required for content-complete beta (multiple series, mod pipeline, balance pass)
- **[DEP: X.Y]** = Depends on task X.Y being complete — do not start until dependency is done
- **Exit Criteria** = How you know a task is done — treat as acceptance test

See `CLAUDE.md` section 13 for the full phase dependency tree.

## 9. Documentation Rules

Documentation is part of done criteria.

Update docs in the same PR when changing:

- architecture/layer boundaries,
- public interfaces/contracts,
- persistence/data model behavior,
- CI/build/release process,
- contributor workflow or governance.

Minimum files to consider:

- `Documentation/Architecture/*`
- `Documentation/Development/*`
- `Documentation/Operations/*`
- `Documentation/README.md`
- new ADR in `Documentation/ADR/` when decisions include meaningful trade-offs

Use templates:

- ADR template: `Documentation/Templates/adr-template.md`
- Feature spec template: `Documentation/Templates/feature-spec-template.md`

## 10. Lookup Table (If You Need X, Read Y)

- Game design pillars (non-negotiable rules):
  - `Documentation/Product/01-game-pillars.md`
- Project intent and scope:
  - `Documentation/Product/00-project-overview.md`
- Design principles, phase map, guardrails:
  - `CLAUDE.md`
- Layering constraints:
  - `Documentation/Architecture/01-layering-and-rules.md`
- Package responsibilities:
  - `Documentation/Architecture/02-package-catalog.md`
- Bootstrap and composition root:
  - `Documentation/Architecture/03-runtime-bootstrap-and-composition.md`
- Persistence principles:
  - `Documentation/Architecture/04-data-and-persistence.md`
- Coding standards:
  - `Documentation/Development/02-coding-standards.md`
- Workflow and PR policy:
  - `Documentation/Development/01-workflow-and-doc-updates.md`
- Testing expectations:
  - `Documentation/Development/03-testing-strategy.md`
- CI behavior and branch-protection alignment:
  - `Documentation/Operations/00-ci-cd.md`
- Governance decisions:
  - `Documentation/ADR/0001-architecture-layering.md`
  - `Documentation/ADR/0002-protected-main-and-pr-workflow.md`

## 11. Validation Expectations

Before opening or updating a PR:

1. Ensure asmdef/package references still obey architecture direction.
2. Ensure no `UnityEngine` usage leaked into engine-free packages.
3. Ensure no magic numbers — all tuning values from definition files.
4. Ensure no `float`/`double` arithmetic in Sim layer hot paths.
5. Ensure no mutable statics or singletons introduced.
6. Ensure docs were updated for any process/architecture changes.
7. Run local checks appropriate to the change (at minimum compile sanity).
8. Verify CI/workflow implications are acknowledged in PR description.

## 12. Security and Safety Baselines

- Never commit secrets, tokens, license files, or credentials.
- Use Git LFS rules already defined in `.gitattributes`.
- Do not change repository visibility, branch protection, or secrets unless explicitly requested.

## 13. When Blocked

If blocked by CI, licensing, or governance settings:

1. Capture the exact failing step/log snippet.
2. Document whether failure is code, infrastructure, or policy.
3. Propose smallest safe unblock that keeps PR-only governance intact.
4. Create/attach a tracking issue if the fix is deferred.

Current known blockers and trackers:

- Unity activation strategy for GameCI: issue `#2`
- Post-CI-stability gameplay slice planning: issue `#3`

