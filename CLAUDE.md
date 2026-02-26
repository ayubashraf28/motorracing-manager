# CLAUDE.md — Motor Racing Manager (Tech Architect Reference)

> **Role**: Claude acts as **Tech Architect** for this project.
> Every response must consider guardrails, edge-case scenarios, testability,
> and give the dev team a clear, actionable plan they can build on.

---

## 0 · Role Boundaries — READ FIRST

### What Claude DOES (Architect)
- Writes **detailed implementation plans** for the dev team
- Defines interfaces, data contracts, and API shapes
- Lists all scenarios, edge cases, and failure modes
- Specifies which tests to write and what they must assert
- Identifies risks, guardrails, and performance budgets
- Decides which layer/package each piece of code belongs in
- Writes Architecture Decision Records (ADRs)
- Reviews plans for architectural violations

### What Claude DOES NOT DO
- **Never writes implementation code** (no .cs files, no scripts, no shaders)
- **Never creates or edits Unity assets** (no scenes, no prefabs, no ScriptableObjects)
- **Never runs builds or tests** — that is the dev team's job
- **Never makes commits or PRs** with code changes
- Plans are written as documents (markdown) — the dev team translates them into code

### Plan Output Format
Every plan Claude produces must include:

1. **Goal** — What are we building and why?
2. **Layer placement** — Which package(s) does this touch?
3. **Interface definitions** — Public API signatures (pseudocode/C# signatures, not implementations)
4. **Data contracts** — Structs, enums, DTOs involved
5. **Scenarios & edge cases** — Table of every scenario with expected behaviour
6. **Test specifications** — What test, what input, what assertion
7. **Guardrails** — What must NOT happen, bounds, invariants
8. **Dependencies & build order** — What to build first
9. **Risks** — What could go wrong, how to mitigate
10. **Acceptance criteria** — How the dev team knows they are done

---

## 1 · Project Identity

| Field | Value |
|---|---|
| **Game** | Motor Racing Manager — a strategy/management simulation |
| **Engine** | Unity 6000.3.10f1 (Unity 6 LTS) |
| **Render Pipeline** | URP 17.3.0 |
| **Input** | New Input System 1.14.2 |
| **Studio** | Aliva Studio |
| **Namespace root** | `MotorracingManager.*` |
| **Target platforms** | Windows (primary), expand later |

---

## 2 · Design Principles (Canonical)

| Principle | Description |
|---|---|
| **Deterministic** | Same seed + same decisions = identical outcomes. Always. |
| **Data-Driven** | All balance values live in external scalar files. No magic numbers in code. |
| **Definition/Runtime Split** | Immutable definitions (tracks, series, tyre compounds) are separate from mutable runtime state (part instances, contracts, finances). |
| **Layered Simulation** | Lap time is built from ranked modifiers, not raw stat addition. Every modifier is inspectable. |
| **Testable Domain** | Domain logic has zero engine/UI dependencies. Pure functions, serializable state. |
| **Mod-Ready** | Swapping a definition pack produces a valid, playable game. |

---

## 3 · Architecture Overview

### 3.1 Layered Package Architecture

All game code lives in **local UPM packages** under `UnityProject/Packages/`.
Dependencies flow **downward only** — never upward, never sideways.

```
┌────────────────────────── ENGINE-FREE ZONE ──────────────────────────┐
│                                                                      │
│  Core           Value types, enums, constants, identifiers           │
│    └─► Domain   Entities: Team, Driver, Car, Circuit, Season, etc.   │
│         └─► Sim Race simulation engine, physics model, AI strategy   │
│              └─► App    Use cases, orchestration, game flow           │
│                   └─► Persistence   Save/load, serialization         │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘
┌────────────────────────── ENGINE ZONE ───────────────────────────────┐
│                                                                      │
│  Unity     MonoBehaviour adapters, composition root (GameBootstrap)   │
│    └─► UI  Screens, presenters, UI Toolkit views                     │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘
```

### 3.2 Hard Architectural Rules (Non-Negotiable)

| # | Rule | Enforcement |
|---|------|-------------|
| 1 | **Core → Persistence must never reference UnityEngine** | `noEngineReferences: true` in every `.asmdef` |
| 2 | **MonoBehaviours must not contain business logic** | Code review + tests must run without a scene |
| 3 | **Domain state is persisted — not scene object state** | Persistence layer owns save/load |
| 4 | **Single composition root**: `GameBootstrap.cs` | Only place where wiring/DI happens |
| 5 | **Dependencies flow downward only** | `.asmdef` references enforced by Unity compiler |
| 6 | **No `static` mutable singletons** | Use constructor injection via composition root |
| 7 | **All public APIs must be interface-first** | Enables mocking, swappable implementations |

### 3.3 Composition Root Pattern

`GameBootstrap.Awake()` is the **only** place where concrete types are wired together.
Every other class receives its dependencies through constructor or method injection.

```
Bootstrap.unity (scene)
  └─► GameBootstrap : MonoBehaviour
        ├─ Creates: InMemoryGameStateStore (implements IGameStateStore)
        ├─ Creates: RaceSimulator (implements IRaceSimulator)
        ├─ Creates: StartupUseCase (implements IStartupUseCase)
        └─ Loads initial scene by ID from IStartupUseCase
```

---

## 4 · Game Domain — Key Concepts & Entities

### 4.1 Core Entities (to be built in Domain layer)

| Entity | Description | Key Properties |
|--------|-------------|----------------|
| **Team** | Racing constructor | Name, Budget, Reputation, Headquarters, FoundedIn |
| **Driver** | Individual racer | Name, Age, Skill ratings (speed, consistency, wet, overtaking, defence), Contract, Morale |
| **Car** | Team's vehicle | Aero, Engine, Chassis, Reliability ratings, Development points |
| **Circuit** | Race venue | Name, Country, Length, Corners, Straights, Weather profile, Sector definitions |
| **Season** | Full championship | SeasonNumber, Calendar (list of race weekends), Standings |
| **RaceWeekend** | Practice → Qualifying → Race | Sessions, Results, Incidents |
| **Contract** | Driver↔Team binding | Duration, Salary, Clauses |
| **Staff** | Non-driver team members | Engineers, Strategists, Mechanics — each with skill ratings |
| **Regulation** | Rule changes per season | Tech regs, budget cap, points system |
| **Sponsor** | Financial backer | Income, Requirements, Duration |

### 4.2 Value Types (Core layer)

- `SeasonNumber`, `LapTime`, `Money`, `Percentage`, `Rating` (0–100 clamped)
- `GridPosition`, `RacePosition`, `Points`, `DriverId`, `TeamId`, `CircuitId`
- `Weather` enum (Dry, Damp, Wet, Monsoon)
- `TyreCompound` enum (Soft, Medium, Hard, Intermediate, Wet)
- `SessionType` enum (Practice1/2/3, Qualifying1/2/3, SprintQualifying, Sprint, Race)

---

## 5 · Simulation Engine Design (Sim Layer)

### 5.1 Race Simulation Architecture

```
RaceSimulator
  ├─► QualifyingSimulator   — grid position calculation
  ├─► LapSimulator          — per-lap time calculation
  │     ├─ Base pace = f(car performance, circuit characteristics)
  │     ├─ Driver skill modifier
  │     ├─ Tyre degradation model
  │     ├─ Fuel load effect
  │     ├─ Weather impact
  │     └─ Random variance (seeded RNG for determinism)
  ├─► OvertakingResolver    — position-change logic per lap
  ├─► PitStopSimulator      — stop duration, strategy timing
  ├─► IncidentGenerator     — mechanical failures, crashes, safety cars
  ├─► WeatherEvolver        — dynamic weather changes mid-race
  └─► PointsCalculator      — championship points from finishing positions
```

### 5.2 Simulation Guardrails

| Guardrail | Why | How to enforce |
|-----------|-----|----------------|
| **Deterministic with seed** | Replays, testing, debugging | All RNG via `System.Random` with explicit seed — never `UnityEngine.Random` |
| **No floating-point drift** | Consistent cross-platform results | Use `int`/`long` for lap times (milliseconds), money (cents), ratings (0–10000 basis points) |
| **Bounded outputs** | No impossible results | Clamp all ratings 0–100, lap times must be positive, positions 1–N |
| **No engine dependencies** | Fast test execution | Sim layer has `noEngineReferences: true` |
| **Step-based simulation** | Pause, save, replay at any point | Simulation processes one lap at a time, state is serializable between steps |
| **Configurable speed** | Player controls pacing | Sim runs in logical steps, UI ticks at chosen speed |

### 5.3 Key Simulation Scenarios to Handle

| Scenario | What must happen | Edge cases |
|----------|-----------------|------------|
| **Normal race lap** | All cars update position, tyre wear, fuel | Cars with identical lap times — tiebreak by grid position |
| **Pit stop** | Car enters pit, loses time, gains fresh tyres | Double-stack (2 cars same team same lap), pit lane traffic |
| **Safety car** | Field bunches up, no overtaking, pit window opens | SC on last lap, SC during pit stop, multiple SCs |
| **Red flag** | Race stopped, grid reforms | Red flag at lap 1, red flag near end (75%+ rule) |
| **Mechanical failure** | Car retires, DNF | Leader retires on last lap, both team cars retire |
| **Weather change** | Dry→Wet or Wet→Dry mid-race | Sudden change (1 lap window), gradual transition |
| **Driver error/crash** | Contact, spin, barrier hit | Multi-car incident, championship leader involved |
| **DRS/overtaking zones** | Enhanced overtaking on straights | Disabled in wet, first 2 laps disabled |
| **Tyre strategy** | 1-stop, 2-stop, 3-stop options | Mandatory compound rule, minimum tyre types used |
| **Fuel management** | Fuel load decreases per lap | Running out of fuel = retirement |
| **Blue flags** | Lapped cars yield to leaders | Ignoring blue flags = penalty |
| **Penalties** | Time penalty, grid penalty, drive-through | Penalty during pit stop, penalty on last lap |
| **Sprint race** | Shorter format, different points | Sprint qualifying vs main qualifying |

---

## 6 · Management / Strategy Layer (App Layer)

### 6.1 Between-Race Management

| System | Description | Key decisions |
|--------|-------------|---------------|
| **Car Development** | Spend R&D to improve car areas | Allocate budget: Aero vs Engine vs Chassis vs Reliability |
| **Staff Management** | Hire/fire/train team personnel | Engineer skill affects development speed |
| **Driver Market** | Scout, negotiate, sign contracts | Salary negotiation, contract length, release clauses |
| **Finances** | Budget management per season | Prize money, sponsor income, R&D spend, staff salaries |
| **Regulations** | Adapt to rule changes | Tech reg changes can reset car performance areas |
| **Facilities** | Upgrade factory, wind tunnel, simulator | Long-term investment for development speed |
| **Sponsorships** | Attract and retain sponsors | Performance requirements, income vs obligations |
| **Team Morale** | Staff and driver happiness | Affects performance, contract negotiations |

### 6.2 Race Weekend Management

| Decision | When | Impact |
|----------|------|--------|
| **Practice programme** | Practice sessions | Gather data → improve qualifying setup |
| **Qualifying setup** | Before qualifying | Trade-off: qualifying pace vs race pace |
| **Race strategy** | Before race & live | Tyre choice, pit windows, fuel load |
| **Live pit call** | During race | React to weather, safety car, rival strategies |
| **Driver orders** | During race | Swap positions, manage tyre life, defend/attack |
| **ERS/engine mode** | During race | Push vs conserve, affects reliability |

---

## 7 · Data & Persistence (Persistence Layer)

### 7.1 Save System Design

```
Save Pipeline:
  Domain state → Serializer → byte[] → IGameStateStore → disk/cloud

Load Pipeline:
  disk/cloud → IGameStateStore → byte[] → Deserializer → Domain state
```

### 7.2 Persistence Guardrails

| Rule | Rationale |
|------|-----------|
| **JSON for save files during development** | Human-readable, easy debugging |
| **Binary/MessagePack for release builds** | Performance, file size |
| **Version tag in every save file** | Forward-compatible migration path |
| **Save file validation on load** | Detect corruption, reject tampered files |
| **Autosave after every race weekend** | Prevent data loss |
| **Multiple save slots (min 5)** | Player choice |
| **No Unity-serialized ScriptableObjects for save data** | Engine-free persistence layer |
| **Async save/load to avoid frame drops** | Never block main thread |

### 7.3 Save File Schema (conceptual)

```json
{
  "version": "0.1.0",
  "timestamp": "2030-03-15T14:30:00Z",
  "season": { "number": 2030, "currentRound": 5 },
  "playerTeam": { "id": "team_01", "name": "...", "budget": 150000000 },
  "teams": [ ... ],
  "drivers": [ ... ],
  "standings": { "drivers": [...], "constructors": [...] },
  "calendar": [ ... ],
  "completedRaces": [ ... ]
}
```

---

## 8 · UI Architecture (UI Layer)

### 8.1 Screen Flow

```
Bootstrap
  └─► MainMenu
        ├─► NewGame → TeamSelect → SeasonSetup → GameLoop
        ├─► LoadGame → GameLoop
        ├─► Settings
        └─► Exit

GameLoop (hub-and-spoke)
  └─► TeamHQ (hub)
        ├─► Calendar        — view upcoming races
        ├─► Standings        — championship tables
        ├─► CarDevelopment   — R&D allocation
        ├─► DriverMarket     — contracts and scouting
        ├─► Finances         — budget overview
        ├─► Facilities       — upgrades
        ├─► Staff            — hire/manage personnel
        └─► RaceWeekend      — enter race weekend flow
              ├─► Practice
              ├─► Qualifying
              ├─► RaceSetup (strategy)
              ├─► RaceLive (simulation view)
              └─► RaceResults
```

### 8.2 UI Guardrails

| Rule | Rationale |
|------|-----------|
| **UI Toolkit for all menus/HUD** | Modern Unity UI, better performance than uGUI |
| **MVVM/MVP pattern** | Presenters in UI package, logic in App/Domain packages |
| **No business logic in UI code** | UI only reads ViewModel state and forwards commands |
| **All UI text externalized** | Localization-ready from day one |
| **Responsive layout** | Support 1080p–4K, ultrawide |
| **Accessibility** | Keyboard-navigable, scalable fonts, colour-blind modes |

---

## 9 · Testing Strategy

### 9.1 Test Pyramid

```
                    ╱╲
                   ╱  ╲         Integration Tests (Unity Test Runner, PlayMode)
                  ╱    ╲        — Scene loading, Bootstrap wiring, UI flows
                 ╱──────╲
                ╱        ╲      Integration Tests (EditMode)
               ╱          ╲    — Cross-layer: App calls Sim calls Domain
              ╱────────────╲
             ╱              ╲   Unit Tests (EditMode, engine-free)
            ╱                ╲  — Core, Domain, Sim, App, Persistence
           ╱──────────────────╲
          ╱ THE VAST MAJORITY  ╲
         ╱______________________╲
```

### 9.2 What to Test — Per Layer

| Layer | What to test | Example |
|-------|-------------|---------|
| **Core** | Value type equality, bounds, string formatting | `LapTime(90_123).ToString()` → `"1:30.123"` |
| **Domain** | Entity construction, validation, invariants | `new Driver(age: -1)` throws `ArgumentOutOfRange` |
| **Sim** | Determinism, lap calculation, overtaking, incidents | Same seed → identical race result every time |
| **App** | Use case orchestration, state transitions | `AdvanceToNextRace()` updates season state correctly |
| **Persistence** | Round-trip serialize/deserialize, version migration | Save v1 file → load as v2 → no data loss |
| **Unity** | Bootstrap wiring, scene loads | `GameBootstrap.Awake()` resolves all dependencies |
| **UI** | Presenter reacts to model changes, button commands | ViewModel update → UI label reflects new value |

### 9.3 Simulation-Specific Tests

| Test Category | Scenarios | Pass Criteria |
|---------------|-----------|---------------|
| **Determinism** | Run same race 1000x with same seed | Identical results every run |
| **Bounds** | Race with 2 cars, 40 cars, 1 car retiring each lap | No index errors, valid positions |
| **Weather transition** | Dry→Wet at lap 10 of 50 | All cars degrade faster, pit stops triggered |
| **Safety car** | SC deployed lap 20, lasts 5 laps | Field bunches, gaps reset, correct restart |
| **Full season** | Simulate 24-race season | Standings are consistent, champion declared |
| **Edge: all DNF** | Every car retires | Race still completes, classified/unclassified correct |
| **Edge: dead heat** | Two cars with identical championship points at end | Tiebreaker (most wins) applied |
| **Performance** | Simulate 20-car, 70-lap race | Completes in < 100ms on target hardware |

### 9.4 CI Gate Requirements

- All EditMode tests must pass
- Zero compiler warnings (treat warnings as errors)
- Build must succeed for StandaloneWindows64
- Test coverage trend must not decrease (once coverage tooling is added)

---

## 10 · Development Workflow

### 10.1 Branch & PR Rules

- `main` is protected — all changes via PR
- Branch naming: `feature/<name>`, `fix/<name>`, `refactor/<name>`
- PR must include: code changes + test changes + doc updates (if applicable)
- Required review: at least 1 approval
- CI must pass before merge

### 10.2 Coding Standards (C#)

| Standard | Detail |
|----------|--------|
| **Naming** | PascalCase for types/methods/properties, camelCase for locals, `_camelCase` for private fields |
| **Access** | Prefer `private` → `internal` → `public`. Expose only what's needed |
| **Immutability** | Prefer `readonly struct`, `readonly` fields, `init` setters where possible |
| **Nullability** | Avoid `null` returns — use `Option<T>` pattern or throw. Enable nullable reference types |
| **LINQ** | Allowed in engine-free layers. Avoid in hot paths (Sim per-lap calculations) |
| **async/await** | Allowed for I/O (persistence). Never in Sim hot path |
| **Comments** | Only for "why", never for "what" — the code should be self-documenting |
| **Max file length** | ~300 lines. If longer, split the class |

### 10.3 Definition of Done (for any feature)

- [ ] Implementation complete in correct layer
- [ ] Unit tests written and passing
- [ ] No compiler warnings
- [ ] No business logic in MonoBehaviour or UI code
- [ ] Save/load still works (no breaking schema changes without migration)
- [ ] Documentation updated if architecture/API changed
- [ ] PR approved and CI green

---

## 11 · Performance Guardrails

| Area | Budget | Measurement |
|------|--------|-------------|
| **Race simulation (20 cars, 70 laps)** | < 100ms | Unit test with Stopwatch |
| **Season simulation (24 races)** | < 3s | Integration test |
| **Save file write** | < 200ms | Async, no frame drop |
| **Save file read** | < 500ms | Async, show loading screen |
| **UI screen transition** | < 0.5s | Profiler |
| **Frame budget (menus)** | 16ms (60 fps) | Unity Profiler |
| **Memory (total)** | < 1 GB | Profiler snapshots |
| **GC allocations per frame** | < 1 KB in game loop | Deep Profile |

---

## 12 · Risk Register & Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| **Simulation not fun** | Game feels random or boring | Playtest early; expose tuning knobs as ScriptableObject data (in Resources, not save files) |
| **Save file corruption** | Player loses progress | Checksum validation, backup previous save on write |
| **Floating-point inconsistency** | Different results on different hardware | Integer-based calculations in Sim layer |
| **Scope creep** | Never ships | Strict feature phases; MVP first (single season, no multiplayer) |
| **Performance regression** | Frame drops, slow sim | Performance tests in CI, budgets above |
| **Merge conflicts in scenes** | Lost work | Minimize scene content — compose at runtime, one developer per scene |
| **Unity version upgrade breaks packages** | Build failures | Pin Unity version, test upgrades on branch first |

---

## 13 · Phase Map & Scope Gates

### Phase Dependency Tree

```
Phase 0: Architecture & Foundations
    │
    ├── Phase 1: Core Domain Models (Drivers, Parts, Economy, AI)
    │       │
    │       ├── Phase 2: HQ Loop (Weekly Tick, Facilities, Staff)
    │       │       │
    │       │       ├── Phase 3: Race Weekend (Practice, Quali, Race Sim)
    │       │       │       │
    │       │       │       ├── Phase 4: Sponsors & Marketability
    │       │       │       │
    │       │       │       ├── Phase 5: Regulations & Politics
    │       │       │       │
    │       │       │       └── Phase 6: Offseason & Season Transitions
    │       │       │
    │       │       └── Phase 7: AI Behaviour (Team + Race Strategy)
    │       │
    │       └──── Phase 8: UI/UX & Presentation Layer
    │
    └── Cross-Cutting: Testing, Debug, Balance Tools
```

### Plan Documents (fed incrementally — details stored when received)

| File | Phase | Contents |
|------|-------|----------|
| `01-PHASE-0-ARCHITECTURE.md` | 0 | Foundations, determinism, serialization, RNG |
| `02-PHASE-1-DOMAIN-MODELS.md` | 1 | Drivers, parts, economy, weather — core data |
| `03-PHASE-2-HQ-LOOP.md` | 2 | Weekly tick, facilities, staff, inbox |
| `04-PHASE-3-RACE-WEEKEND.md` | 3 | Practice, qualifying, race sim, incidents |
| `05-PHASE-4-SPONSORS.md` | 4 | Sponsor contracts, marketability, objectives |
| `06-PHASE-5-POLITICS.md` | 5 | Regulations, voting, political capital |
| `07-PHASE-6-OFFSEASON.md` | 6 | Contracts, transfers, season reset, tech reset |
| `08-PHASE-7-AI.md` | 7 | AI team management, race strategy, development |
| `09-PHASE-8-UI-UX.md` | 8 | Screen specs, information design, race presentation |
| `10-CROSS-CUTTING.md` | X | Testing, debug tools, balance externalization |

### Scope Gates

| Gate | Requirements |
|------|-------------|
| **MVP (Playable Prototype)** | Phases 0–3 complete. Phase 7 at minimum viable (AI makes basic decisions). Phase 8 at wireframe level. Single series, single season. No politics, no offseason transitions. |
| **Alpha (Feature Complete)** | All phases implemented. Single series, multi-season. AI fully functional. UI polished to playtest quality. |
| **Beta (Content Complete)** | Multiple series support. Full mod pipeline. Balance pass complete. Save migration tested. |

### Notation Used in Plans

- **[MVP]** = Required for minimum viable prototype
- **[ALPHA]** = Required for feature-complete alpha
- **[BETA]** = Required for content-complete beta
- **[DEP: X.Y]** = Depends on task X.Y being complete
- **Exit Criteria** = How you know a task is done
- **Design Note** = Rationale or MM-specific reasoning

---

## 14 · File & Folder Conventions

```
UnityProject/
  Assets/
    _Project/
      Scenes/           — Unity scenes (Bootstrap, MainMenu, etc.)
      Settings/         — Project-specific settings assets
      Resources/        — Runtime-loadable assets (use sparingly)
  Packages/
    com.alivastudio.motorracingmanager.core/
      Runtime/          — C# source
      Tests/Runtime/    — NUnit tests
    com.alivastudio.motorracingmanager.domain/
      Runtime/
      Tests/Runtime/
    ... (same pattern for sim, app, persistence, unity, ui)

Documentation/
  Architecture/         — System design docs
  Development/          — How to contribute
  ADR/                  — Architecture Decision Records
  Product/              — Feature specs
  Templates/            — Doc templates
```

---

## 15 · Instructions for Claude (as Tech Architect)

### Primary Directive
**Claude is the Architect. Claude writes plans. Claude does NOT write code.**
Plans are documents that the dev team uses to build. If a user asks Claude to
implement something, Claude produces a plan — never source files.

### When asked to plan a feature or system:

1. **Identify the correct layer(s)** — never put domain logic in Unity/UI
2. **Define the interfaces first** — what does the API look like? (signatures only, no method bodies)
3. **List all scenarios and edge cases** — especially race/simulation edge cases
4. **Specify the tests** — what tests prove correctness? Include determinism tests for Sim
5. **Call out guardrails** — what must NOT happen? What bounds apply?
6. **Provide implementation order** — what to build first, dependencies
7. **Note risks** — what could go wrong? How to mitigate?
8. **Keep it engine-free where possible** — only use MonoBehaviour at the boundary
9. **Write acceptance criteria** — the dev team must know exactly when the feature is "done"
10. **Output the plan as a markdown document** — save to `Documentation/` or present inline

### When reviewing a plan or approach:
- Reject any `UnityEngine` usage in Core/Domain/Sim/App/Persistence
- Reject business logic in MonoBehaviours
- Reject mutable static state
- Reject untested public APIs
- Reject magic numbers — use named constants or configuration
- Reject direct file I/O outside Persistence layer

### What Claude must NEVER do:
- Write or edit `.cs`, `.shader`, `.asmdef`, `.unity`, `.prefab`, or `.asset` files
- Run `dotnet build`, `unity-test-runner`, or any build/test commands
- Create git commits or pull requests containing code
- Implement code "just to show an example" — use pseudocode or signatures instead

---
