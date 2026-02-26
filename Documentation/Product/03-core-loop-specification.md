# Core Loop Specification

> Status: LOCKED
> Approved: February 26, 2026
> Depends on: Product/01-game-pillars.md, Product/02-performance-philosophy.md
>
> This document defines the complete game loop - every player action,
> every state mutation, and the exact tick order. All orchestration code
> in the App layer must implement this specification.

## 1. Loop Overview

The game loop is a weekly tick model:
- Time must advance only when the player presses `Advance Week`.
- Each weekly transition must follow this contract:
  `GameState(week N) + PlayerDecisions -> GameState(week N+1)`.
- Tick output must be a deterministic function of current state, queued decisions, and seed.
- A season must span approximately `40-52` weeks (pre-season, race calendar, offseason).
- The player must always be in one of three gameplay modes: `HQ`, `Race Weekend`, or `Offseason`.

Simplified state machine:

```text
         ┌──────────┐
         │ MainMenu │
         └────┬─────┘
              │ New Game / Load Game
              ▼
         ┌──────────┐    Advance Week     ┌──────────┐
    ┌───►│ HQ Mode  │───────────────────►│ Week Tick │
    │    └──────────┘                     └────┬─────┘
    │                                          │
    │         ┌────────────────────────────────┼────────────────┐
    │         │                                │                │
    │         ▼                                ▼                ▼
    │    ┌─────────┐                    ┌───────────┐    ┌───────────┐
    │    │ Normal   │                    │ Race      │    │ Offseason │
    │    │ Week     │                    │ Weekend   │    │ Flow      │
    │    └────┬────┘                    └─────┬─────┘    └─────┬─────┘
    │         │                               │                │
    └─────────┴───────────────────────────────┴────────────────┘
                          Return to HQ
```

## 2. Game States & Transitions

| State | Description | Valid transitions |
|---|---|---|
| `MainMenu` | Title screen, new/load/settings | `-> NewGameSetup`, `-> LoadGame` |
| `NewGameSetup` | Team selection, difficulty, season configuration | `-> HQ` (first week of season) |
| `HQ` | Player home base and decision hub | `-> WeekTick` (advance week), `-> Save`, `-> MainMenu` (quit) |
| `WeekTick` | Non-interactive weekly processing | `-> HQ` (normal week), `-> RaceWeekend` (race week), `-> Offseason` (offseason week) |
| `RaceWeekend` | Practice, qualifying, race, and results flow | `-> HQ` (after results) |
| `Offseason` | Contracts, transfers, regulation effects, season reset | `-> HQ` (first week of new season) |
| `Save/Load` | Serialization and restoration of game state | `-> HQ` (resume) |

Transition rules:
- No state may transition to itself.
- All game-time-advancing transitions must pass through `WeekTick`.
- Navigation transitions (`MainMenu`, `NewGameSetup`, `Save/Load`, quit) are non-time transitions and must not mutate world time.
- The player must never skip weeks, rewind weeks, or jump ahead in calendar time.

## 3. The Weekly Tick - Order of Operations

Weekly tick contract:

```text
WeekTick(currentState, playerDecisions, rngSeed) -> newState
```

Step order is load-bearing and must execute in this exact sequence:

| Step | Name | What it does | Scope gate |
|---:|---|---|---|
| 1 | Apply Player Decisions | Processes all queued player actions from HQ (hiring, release, development orders, voting, and accepted offers). | `[MVP]` |
| 2 | Process Expirations | Expires ended contracts, completed offers, and time-limited opportunities. | `[MVP]` |
| 3 | Advance Build Queues | Decrements remaining weeks on all active build and upgrade queues. | `[MVP]` |
| 4 | Complete Builds | Marks zero-week builds complete and applies resulting capability changes. | `[MVP]` |
| 5 | Apply Weekly Finances | Applies weekly income and cost flows (salaries, operations, sponsor income, installments). | `[MVP]` |
| 6 | Update Part Wear | Applies part wear and fatigue progression for active race equipment. | `[MVP]` |
| 7 | Process Staff Effects | Applies morale, training, and staff progression effects. | `[ALPHA]` |
| 8 | Generate AI Decisions | Generates AI team decisions for staffing, development, and strategy setup. | `[MVP]` |
| 9 | Process Market Activity | Applies AI transfer activity and market movements. | `[ALPHA]` |
| 10 | Generate Events | Generates seeded events such as offers, rumors, injuries, and poaching attempts. | `[ALPHA]` |
| 11 | Check Calendar | Determines whether week branch is normal, race weekend, or offseason. | `[MVP]` |
| 12 | Branch | Enters `RaceWeekend` sub-loop or `Offseason` sub-loop when applicable, else continues normal flow. | `[MVP]` |
| 13 | Update Rankings | Recalculates part rankings and updates standings when race results exist. | `[MVP]` |
| 14 | Generate Inbox Messages | Compiles all outcomes, events, financial updates, and actionable notices into inbox items. | `[MVP]` |
| 15 | Increment Week Counter | Increments calendar index by exactly one week. | `[MVP]` |
| 16 | Autosave | Writes autosave snapshot after successful tick completion. | `[MVP]` |

Tick rules:
- Steps `1-16` are atomic: the tick must complete entirely or not apply.
- Tick output must be deterministic for identical inputs.
- Tick processing must remain App-layer orchestration and must not depend on engine lifecycle.
- All RNG-consuming operations must use one shared deterministic sequence in fixed step order.
- Steps marked `[ALPHA]` must remain in the pipeline during MVP as no-op stubs that return neutral results.
- Reordering any step is forbidden without an ADR.

## 4. Player Action Catalog

| # | Action | Available in | Queued or Immediate | State mutation | Validation rules | Scope |
|---:|---|---|---|---|---|---|
| 1 | Hire Staff | `HQ` | Queued -> Step 1 next tick | Adds staff member and deducts signing fee from budget. | Budget must cover fee, role slot must be available, and target must be contract-eligible. | `[MVP]` |
| 2 | Fire Staff | `HQ` | Queued -> Step 1 next tick | Removes staff member, deducts severance, and applies morale impact. | Action blocked during race weekend; severance must follow configured multiplier rules. | `[ALPHA]` |
| 3 | Start Part Development | `HQ` | Queued -> Step 1 next tick | Creates part build order with type, target specification, duration, and cost. | Budget must cover cost, build slot must be available, and concurrent limit must not be exceeded. | `[MVP]` |
| 4 | Cancel Part Development | `HQ` | Queued -> Step 1 next tick | Removes build order and applies partial refund by completion percentage. | Refund must follow configured remaining-cost refund rate. | `[MVP]` |
| 5 | Start Facility Upgrade | `HQ` | Queued -> Step 1 next tick | Creates facility upgrade order with target level and duration. | Budget must cover cost, max level must not be exceeded, and facility must not already be upgrading. | `[ALPHA]` |
| 6 | Accept Sponsor Offer | `HQ` | Queued -> Step 1 next tick | Adds sponsor contract and schedules income flow from next finance step. | Sponsor slots must be available and offer must still be valid. | `[ALPHA]` |
| 7 | Reject Sponsor Offer | `HQ` | Queued -> Step 1 next tick | Removes offer and applies sponsor cooldown window. | No special validation required. | `[ALPHA]` |
| 8 | Sign Driver | `HQ` | Queued -> Step 1 next tick | Assigns driver to seat, creates contract, and deducts signing and salary obligations. | Driver must be available by market rules, seat must be available, and budget must cover obligations. | `[MVP]` |
| 9 | Release Driver | `HQ` | Queued -> Step 1 next tick | Removes driver from seat, deducts buyout fee, and releases driver to market. | Action blocked during race weekend; buyout must match contract terms. | `[ALPHA]` |
| 10 | Set Car Setup | `Race Weekend` (Practice) | Immediate | Updates weekend setup profile for current session flow. | Setup values must remain within regulation-legal bounds. | `[MVP]` |
| 11 | Choose Race Strategy | `Race Weekend` (Pre-Race) | Immediate | Sets pit plan, tire choices, and initial fuel strategy. | Strategy must satisfy compound requirements and stint fuel feasibility. | `[MVP]` |
| 12 | Call Pit Stop | `Race Weekend` (Race) | Immediate (next lap) | Flags selected car to pit at lap end. | Car must not already be in pit sequence; final-lap pit calls are invalid. | `[MVP]` |
| 13 | Change Engine Mode | `Race Weekend` (Race) | Immediate | Changes engine mode profile for selected car. | Mode must be valid for current series/rules. | `[MVP]` |
| 14 | Issue Driver Order | `Race Weekend` (Race) | Immediate | Applies tactical instruction (attack, defend, hold, swap). | Target drivers must be active and not retired. | `[MVP]` |
| 15 | Cast Political Vote | `HQ` (active vote window) | Queued -> Step 1 next tick | Records team vote on active regulation proposal. | Proposal must be active and vote cannot be changed after lock. | `[ALPHA]` |
| 16 | Advance Week | `HQ` | Triggers `WeekTick` | Executes queued actions and advances to next weekly state. | Mandatory preconditions must be satisfied before tick can start. | `[MVP]` |

Action rules:
- HQ actions must be queued and batch-applied at Step 1 of the next tick.
- Race-weekend actions must be immediate and session-local.
- Validation must run both at queue time and apply time.
- Apply-time validation failures must skip the action and generate inbox explanation; tick must continue.
- Reordering or redefining existing action semantics is forbidden without an ADR.

## 5. Race Weekend Sub-Loop

The race weekend branch is entered at Step 12 when calendar state is race week.

```text
Race Weekend Sub-Loop:
  1. Load circuit definition
  2. Set initial weather from circuit weather profile + seed
  3. Practice Session(s)
       ├── Player sets programme (run plans)
       ├── Simulate practice laps -> gather data
       ├── Update track knowledge modifier
       └── Player adjusts setup between sessions
  4. Qualifying Session(s)
       ├── Simulate qualifying laps using lap time formula
       ├── Determine grid positions
       └── Apply qualifying penalties (grid drops)
  5. Pre-Race
       ├── Player confirms strategy (tyres, fuel, pit windows)
       ├── AI teams finalize strategies
       └── Lock setup
  6. Race Simulation (lap-by-lap)
       ├── For each lap:
       │     ├── Calculate lap time per car (full formula)
       │     ├── Resolve overtaking
       │     ├── Process pit stops
       │     ├── Apply incidents/failures
       │     ├── Evolve weather
       │     ├── Check safety car / red flag
       │     ├── Process player commands (pit call, engine mode, orders)
       │     └── Update race state
       └── Race ends when leader completes final lap
  7. Post-Race
       ├── Calculate final positions (apply time penalties)
       ├── Award championship points
       ├── Update championship standings
       ├── Calculate prize money / financial impact
       ├── Update part wear from race stress
       └── Generate race report for inbox
```

Race weekend rules:
- Practice sessions are optional data-gathering; player chooses run programme and receives knowledge progression.
- Qualifying format must come from series definition (single-session, knockout format, or other defined format).
- Race simulation must be lap-step based; each lap is a complete serializable state transition.
- Player race commands take effect on the next lap boundary.
- Weather must evolve via seeded weekend weather model.
- Lap-time calculations must reference [Documentation/Product/02-performance-philosophy.md](d:/Repo/racing-manager/Documentation/Product/02-performance-philosophy.md); the formula must not be duplicated here.

## 6. Offseason Sub-Loop

Offseason flow spans multiple weeks:

```text
Offseason Flow (spans multiple weeks):
  Week S+1: Season End
       ├── Final championship standings locked
       ├── Prize money distributed
       ├── Season awards generated
       └── Player reviews season summary

  Weeks S+2 to S+4: Contract Window
       ├── Expiring contracts flagged
       ├── Driver/staff free agency opens
       ├── AI teams make offers
       ├── Player negotiates renewals and new signings
       └── Transfer market active

  Weeks S+5 to S+6: Regulation Changes
       ├── Voted regulation changes take effect
       ├── Technical resets applied (if major reg change)
       ├── Part performance recalculated under new regs
       └── Teams assess impact

  Weeks S+7 to S+8: Pre-Season Preparation
       ├── New season calendar published
       ├── Pre-season testing (simplified practice)
       ├── Final roster must be locked (2 drivers minimum)
       └── Budget for new season initialized

  Week S+9: Season Start
       └── Return to HQ with Week 1 of new season
```

Offseason rules:
- Offseason flow is `[ALPHA]` scope.
- MVP offseason must use compressed transition with minimal post-season processing.
- New season start requires `2` signed drivers and positive budget; if invalid, forced resolution must apply (auto-sign cheapest available driver and emergency loan fallback).
- Regulation-change mechanics are `[ALPHA]`; MVP keeps regulation set static.

## 7. Inbox & Event System

Inbox categories:

| Category | Examples | Priority | Generated at |
|---|---|---|---|
| `Critical` | Negative budget, expiring mandatory contracts, unresolved blocking requirements | High (popup + inbox) | Step 14 |
| `Results` | Practice report, qualifying grid, race result package | Medium (highlighted inbox) | Post-race in race weekend sub-loop |
| `Offers` | Sponsor offers, staff applications, driver interest | Medium (highlighted inbox) | Step 10 |
| `News` | Rival signing, regulation rumor, injury report | Low (normal inbox) | Step 10 |
| `Financial` | Weekly budget summary, sponsor payment, prize payment | Low (normal inbox) | Step 5 |

Message contract:
- Every message must include: `id`, `week`, `category`, `title`, `body`, and optional `actions`.
- Message generation must be deterministic from state and seed.
- Messages must persist until dismissed or expired; offer expiry window must come from scalar-defined week counts.
- Critical messages must block `Advance Week` until acknowledged.

## 8. State Mutation Rules

| Rule | Description |
|---|---|
| No silent mutations | Every state mutation must map to a documented tick step or explicit player action. |
| Deterministic order | Mutations inside each step must apply in stable order: alphabetical by team ID, then entity ID within team. |
| Budget cannot go below floor | Budget must respect configured minimum debt floor; spending actions crossing floor must be blocked. |
| Roster invariants | Every team must define exactly two driver seats (seats may be vacant in valid windows); staff roles must respect min/max bounds. |
| Calendar is immutable | Season calendar cannot change after season start; cancelled races become no-race weeks and are never removed from schedule shape. |
| Part rankings are recomputed lazily | Rankings must recompute only when part stats change or at race-weekend start, not every tick. |
| All mutations logged | Every mutation must emit trace log entry with week, step, entity, prior value, and new value. Example: `[Week 12, Step 5] Team Apex budget: 45,200,000 -> 44,850,000 (weekly salaries: -350,000)`. |

## 9. Invariants & Bounds

| Invariant | Rule | What breaks if violated |
|---|---|---|
| Tick is atomic | All 16 steps complete or none do. | Partial state corrupts saves and recovery logic. |
| Tick is deterministic | Same state, decisions, and seed produce same output. | Replay and test reproducibility fail. |
| Week counter is monotonic | Week counter increments by exactly one and never decrements. | Calendar progression and event scheduling fail. |
| Budget is bounded | `MIN_BUDGET <= budget <= MAX_BUDGET` must always hold. | Overflow, unbounded debt, or invalid spending states occur. |
| Roster has 2 seats | Every team always has exactly two driver seats defined (may be vacant by rules). | Race entry and lineup assumptions break. |
| Calendar has fixed race count | Season race count is fixed at season creation. | Standings and schedule-derived logic break. |
| No action has undefined output | Every player action maps to explicit mutation behavior. | Behavior becomes untestable and inconsistent. |
| Inbox is append-only within tick | Messages generated during a tick cannot be deleted in the same tick. | Ordering and auditability of communications break. |
| RNG is consumed in fixed order | RNG-consuming steps must advance shared sequence in stable order. | Determinism breaks under equivalent inputs. |

## 10. Verification Strategy

| Test | Input | Expected | What it proves |
|---|---|---|---|
| Tick determinism | Same state, same decisions, seed `42`, run `1000` times. | Byte-identical output state every run. | Pillar 1 deterministic behavior. |
| Empty tick | No queued actions, no race week, no generated events. | Only minimal expected changes occur (finances, wear, week increment). | Tick does not mutate unrelated state. |
| All actions queued | Queue one of each action type in a single tick. | Actions process in order, failures handled safely, state remains valid. | Action catalog completeness and robustness. |
| Race week trigger | Calendar set to race week and advance to that week. | Race weekend sub-loop executes and results are generated. | Branch logic correctness. |
| Offseason trigger | Advance beyond final race week. | Offseason sub-loop executes according to scope behavior. | Branch logic correctness. |
| Budget floor | Queue spending actions that exceed allowed floor. | Overspending actions are rejected and inbox explains rejection. | Budget safety rule enforcement. |
| No drivers for race | Enter race week with vacant required seats. | Forced resolution or defined forfeit path executes with inbox explanation. | Roster invariants and fallback logic. |
| Action validation at apply | Queue action, then invalidate target before apply phase. | Action is skipped at apply time and failure notice is generated. | Double-validation safety. |
| Inbox completeness | Run full `40`-week season simulation. | Every week generates at least one inbox item (minimum financial summary). | No silent week communication gaps. |
| Click count | Measure HQ flow from action selection to `Advance Week`. | Maximum `3` clicks to queue any action and `1` click to advance. | Pillar 7 player-time respect. |
