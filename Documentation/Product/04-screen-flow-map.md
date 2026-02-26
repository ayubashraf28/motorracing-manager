# Screen Flow Map

> Status: LOCKED
> Approved: February 26, 2026
> Depends on: Product/03-core-loop-specification.md
>
> This document defines every screen, its entry/exit conditions,
> required data, and navigation rules. The UI layer implements
> this spec; the App layer exposes the data contracts listed here.

## 1. Navigation Architecture

Two-layer navigation model:
- `System Shell` is always available and exposes Save, Settings, and Quit to Menu.
- `Game Shell` hosts exactly one active game mode at a time: `HQ`, `Weekend`, or `Offseason`.

```text
+---------------------------------------------------------------+
|                         SYSTEM SHELL                          |
|          (always available: Save, Settings, Quit)            |
|                                                               |
|  +---------------------------------------------------------+  |
|  |                        GAME SHELL                       |  |
|  |                                                         |  |
|  |  +-- HQ MODE (tab-based) ----------------------------+  |  |
|  |  | Tab bar: Home | Drivers | Car | ...               |  |  |
|  |  | Content area: active tab content                   |  |  |
|  |  | Footer: Week, Budget, Advance Week                 |  |  |
|  |  +----------------------------------------------------+  |  |
|  |                                                         |  |
|  |  +-- WEEKEND MODE (modal, replaces HQ) --------------+  |  |
|  |  | Session bar: Overview|Practice|Quali|Race|Results  |  |  |
|  |  | Content area: active weekend session               |  |  |
|  |  | Footer: session controls and timing                |  |  |
|  |  +----------------------------------------------------+  |  |
|  |                                                         |  |
|  |  +-- OFFSEASON MODE (sequential, replaces HQ) -------+  |  |
|  |  | Step indicator: Summary|Contracts|Regs|Pre-Season  |  |  |
|  |  | Content area: active offseason step                |  |  |
|  |  +----------------------------------------------------+  |  |
|  |                                                         |  |
|  +---------------------------------------------------------+  |
|                                                               |
+---------------------------------------------------------------+
```

Architecture rules:
- System Shell is always rendered and belongs to UI infrastructure, not game-world progression state.
- Game Shell has one active mode at any point: HQ, Weekend, or Offseason.
- Weekend Mode is modal and fully replaces HQ until weekend completion.
- Offseason Mode is sequential in `[ALPHA]`. MVP uses a single summary fallback screen.

## 2. HQ Screens (Tab-Based)

### 2.1 Home (Dashboard) [MVP]
- Tab name: `Home`
- Purpose: Provide immediate team status overview and quick navigation context.
- Data required: Team name, budget, driver and constructor championship positions, next race name/week, inbox unread count, quick car and driver summary stats.
- Player actions available: No direct state mutations; shortcut navigation only.
- Scope gate: `[MVP]`
- Design rule: This screen must answer "how am I doing?" within 3 seconds and without scrolling.

### 2.2 Drivers [MVP]
- Tab name: `Drivers`
- Purpose: Manage race lineup and driver contracts.
- Data required: Driver roster, ratings, contract terms, morale, recent results, free-agent list, negotiation state.
- Player actions available: `Sign Driver`, `Release Driver`, view comparison, start renewal flow.
- Scope gate: `[MVP]`

### 2.3 Car (Parts + Setup) [MVP]
- Tab name: `Car`
- Purpose: Manage car performance state, development queue, and setup presets.
- Data required: Part category performance and rank, active development orders, reliability values, setup preset list.
- Player actions available: `Start Part Development`, `Cancel Part Development`, create/edit/delete setup presets.
- Scope gate: `[MVP]`
- Sub-views: `Parts`, `Development Queue`, `Setup Editor`.

### 2.4 Staff [ALPHA]
- Tab name: `Staff`
- Purpose: Manage non-driver team personnel.
- Data required: Staff roster, role assignments, ratings, morale, contracts, available hires.
- Player actions available: `Hire Staff`, `Fire Staff`, role assignment updates.
- Scope gate: `[ALPHA]`
- MVP release behavior: Hidden in MVP release builds.

### 2.5 Sponsors [ALPHA]
- Tab name: `Sponsors`
- Purpose: Manage sponsor contracts and incoming offers.
- Data required: Active contracts, objectives, progress tracking, pending sponsor offers.
- Player actions available: `Accept Sponsor Offer`, `Reject Sponsor Offer`, review objective requirements.
- Scope gate: `[ALPHA]`
- MVP release behavior: Hidden in MVP release builds.

### 2.6 Facilities [ALPHA]
- Tab name: `Facilities`
- Purpose: View and manage facility upgrade progression.
- Data required: Facility levels, upgrade effects, costs, durations, active upgrade status.
- Player actions available: `Start Facility Upgrade`.
- Scope gate: `[ALPHA]`
- MVP release behavior: Hidden in MVP release builds.

### 2.7 Calendar [MVP]
- Tab name: `Calendar`
- Purpose: Provide season schedule, completed results, and upcoming event context.
- Data required: Season calendar, completed race results, current week marker, upcoming race metadata.
- Player actions available: Read-only inspection and drill-down only.
- Scope gate: `[MVP]`

### 2.8 Inbox [MVP]
- Tab name: `Inbox`
- Purpose: Present messages, required acknowledgments, and actionable offers.
- Data required: Inbox messages sorted by week, category filters, unread state, offer expiration state.
- Player actions available: Acknowledge critical messages, `Accept/Reject` offers, dismiss messages.
- Scope gate: `[MVP]`
- Design rule: Critical items also surface as HQ-entry popup/toast.

### 2.9 Politics [ALPHA]
- Tab name: `Politics`
- Purpose: Manage active proposals and team vote decisions.
- Data required: Active proposals, vote tallies, vote deadlines, team political capital, regulation history.
- Player actions available: `Cast Political Vote`.
- Scope gate: `[ALPHA]`
- MVP release behavior: Hidden in MVP release builds.

### 2.10 Finances [MVP]
- Tab name: `Finances`
- Purpose: Show budget status, cashflow breakdown, and projections.
- Data required: Current budget, weekly income sources, weekly expenses, projected season-end balance, budget history graph data.
- Player actions available: Read-only; financial mutations are initiated from other tabs.
- Scope gate: `[MVP]`

MVP visibility rule:
- MVP release shows 6 HQ tabs (`Home`, `Drivers`, `Car`, `Calendar`, `Inbox`, `Finances`) plus `Advance Week` in footer.
- ALPHA HQ tabs are defined here for forward scope but are not visible in MVP release builds.

## 3. Race Weekend Screens (Modal Flow)

Weekend flow sequence:

```text
Pre-Race Overview -> Practice 1 -> [Practice 2] -> [Practice 3]
                 -> Qualifying -> Pre-Race Strategy -> Live Race
                 -> Results + Post-Race -> Return to HQ
```

Weekend rules:
- Weekend flow is sealed modal navigation.
- Completed sessions can be reviewed read-only, but cannot be re-run.
- Save is available between any two laps in Live Race.
- Lap time computation references Product/02 performance philosophy and is not duplicated here.

### 3.1 Pre-Race Overview [MVP]
- Purpose: Orient player with circuit context, forecast, and weekend starting state.
- Data required: Circuit metadata, weather forecast, prior setup baseline, circuit knowledge state.
- Actions: Read-only overview actions.
- Entry condition: Weekend begins.
- Exit condition: Player proceeds to Practice.

### 3.2 Practice Session [MVP]
- Purpose: Gather knowledge and tune setup using run plans.
- Data required: Knowledge percentage, setup state, run outputs, tire and fuel telemetry, weather state.
- Actions: Set run program, adjust setup between runs, end session early.
- Entry condition: After overview or prior practice session.
- Exit condition: Session ended by player or session timeout.
- Scope extension: Practice 2 and 3 are `[ALPHA]` optional sessions.

### 3.3 Qualifying [MVP]
- Purpose: Determine race grid order.
- Data required: Qualifying lap outputs, weather, penalties, resulting grid order.
- Actions: Select tire compound and engine mode between runs.
- Entry condition: After final practice session.
- Exit condition: Session completes and grid locks.
- Scope extension: Knockout format (`Q1/Q2/Q3`) is `[ALPHA]`; MVP uses single-session qualifying.

### 3.4 Pre-Race Strategy [MVP]
- Purpose: Finalize race strategy before lights out.
- Data required: Grid order, race forecast, tire options, fuel calculator, partial rival strategy signals.
- Actions: Set pit windows, compounds by stint, and starting fuel load.
- Entry condition: After qualifying.
- Exit condition: Strategy confirmed.

### 3.5 Live Race [MVP]
- Purpose: Run lap-by-lap race simulation with tactical player input.
- Data required: Position tower, lap times, tire state, own fuel state, weather state, timing gaps, pit recommendations.
- Actions: `Call Pit Stop`, `Change Engine Mode`, `Issue Driver Order`.
- Entry condition: After strategy confirmation.
- Exit condition: Race leader completes final lap.
- Presentation contract: Position list, timing panel, event log, weather indicator, speed controls (`1x`, `2x`, `4x`, event-jump).

### 3.6 Results + Post-Race [MVP]
- Purpose: Show outcomes and championship/finance impact.
- Data required: Final classification, points awarded, updated standings, prize and wear impacts, notable event summary.
- Actions: Read-only acknowledgment.
- Entry condition: Race completion.
- Exit condition: Player acknowledges results and returns to HQ.

## 4. Offseason Screens

Offseason screens are `[ALPHA]` sequential flow:

### 4.1 Season Summary [ALPHA]
- Purpose: Review completed season at high level.
- Data: Final standings, season statistics, budget delta, best and worst race snapshots.

### 4.2 Contract Window [ALPHA]
- Purpose: Handle contract expirations and signings.
- Data: Expiring contracts, free agents, AI offer activity, negotiation context.

### 4.3 Regulation Changes [ALPHA]
- Purpose: Review and assess incoming rule changes.
- Data: Regulation change descriptions and impact assessment per performance domain.

### 4.4 Pre-Season [ALPHA]
- Purpose: Finalize roster and preparation before next season begins.
- Data: New calendar, roster lock status, new-season budget state.

MVP fallback:
- Single `Season Complete` summary screen with auto-transition to next season HQ entry.

## 5. System Screens (Menu, Save/Load, Settings)

### 5.1 Main Menu [MVP]
- Buttons: `New Game`, `Load Game`, `Settings`, `Quit`.
- Data state: No active game-world state required.

### 5.2 New Game Setup [MVP]
- Steps: Choose series (MVP single-series), choose team, confirm, start.
- Data required: Series definitions, team definitions, difficulty options.

### 5.3 Save/Load [MVP]
- Slots: Minimum 5 manual save slots plus 1 autosave slot.
- Save rule: Save is available at any point, including between race laps.
- Load rule: Load restores exact point-in-time state (HQ week, weekend session, or race lap boundary).
- Data required: Slot metadata (team, season, week, timestamp, playtime).

### 5.4 Settings [MVP]
- Categories: Simulation speed defaults, audio, display, accessibility.
- Persistence: Applied immediately and stored in player preferences (not season save data).

## 6. Screen Catalog (per-screen spec)

| # | Screen | Mode | Scope | Entry condition | Exit condition | Actions available |
|---:|---|---|---|---|---|---|
| 1 | Main Menu | System | `[MVP]` | App launch or quit from active game | New game or load selected | None |
| 2 | New Game Setup | System | `[MVP]` | Main Menu -> New Game | Team and options confirmed | Choose series, choose team |
| 3 | Home (Dashboard) | HQ | `[MVP]` | Default tab on HQ entry | Tab switch | None (read-only) |
| 4 | Drivers | HQ | `[MVP]` | Tab switch | Tab switch | Sign, release, renew |
| 5 | Car | HQ | `[MVP]` | Tab switch | Tab switch | Start/cancel development, setup preset management |
| 6 | Staff | HQ | `[ALPHA]` | Tab switch (ALPHA builds) | Tab switch | Hire, fire, assign roles |
| 7 | Sponsors | HQ | `[ALPHA]` | Tab switch (ALPHA builds) | Tab switch | Accept, reject, review terms |
| 8 | Facilities | HQ | `[ALPHA]` | Tab switch (ALPHA builds) | Tab switch | Start upgrade |
| 9 | Calendar | HQ | `[MVP]` | Tab switch | Tab switch | Read-only drill-down |
| 10 | Inbox | HQ | `[MVP]` | Tab switch | Tab switch | Acknowledge, accept/reject, dismiss |
| 11 | Politics | HQ | `[ALPHA]` | Tab switch (ALPHA builds) | Tab switch | Cast vote |
| 12 | Finances | HQ | `[MVP]` | Tab switch | Tab switch | None (read-only) |
| 13 | Pre-Race Overview | Weekend | `[MVP]` | Weekend starts | Proceed to practice | None (read-only) |
| 14 | Practice | Weekend | `[MVP]` | After overview or prior practice | End session or timeout | Set program, adjust setup |
| 15 | Qualifying | Weekend | `[MVP]` | After final practice | Session completion | Choose tire, choose engine mode |
| 16 | Pre-Race Strategy | Weekend | `[MVP]` | After qualifying | Strategy confirmed | Set race strategy |
| 17 | Live Race | Weekend | `[MVP]` | After strategy confirmation | Race complete | Pit call, engine mode, driver orders |
| 18 | Results | Weekend | `[MVP]` | After race completion | Acknowledge return to HQ | None (read-only) |
| 19 | Season Summary | Offseason | `[ALPHA]` | After final race | Proceed to contracts | None (read-only) |
| 20 | Contract Window | Offseason | `[ALPHA]` | After summary | Proceed to regulation changes | Sign, release, renew |
| 21 | Regulation Changes | Offseason | `[ALPHA]` | After contracts | Proceed to pre-season | None (read-only) |
| 22 | Pre-Season | Offseason | `[ALPHA]` | After regulation changes | Confirm new season start | Confirm roster |
| 23 | Save/Load | System | `[MVP]` | System shell save/load action | Resume or load complete | Save, load, delete slot |
| 24 | Settings | System | `[MVP]` | System shell settings action | Close settings | Change preferences |

## 7. Navigation Rules & Constraints

| # | Rule | Rationale |
|---:|---|---|
| 1 | Weekend is modal; player cannot return to HQ mid-weekend. | Prevents exploitation of weekend state and preserves sealed weekend progression integrity. |
| 2 | Outside weekend, all MVP HQ tabs are always accessible. | Supports Pillar 7 by minimizing friction and hidden navigation gates. |
| 3 | Save is available at any point, including between race laps. | Prevents progress loss and supports deterministic continuation at lap boundaries. |
| 4 | Setup presets persist across weekends. | Prevents repetitive setup re-entry and preserves player time. |
| 5 | Track knowledge persists within weekend only; inter-season retention is scalar-defined partial carry. | Preserves intended race-week learning arc while allowing controlled long-term retention. |
| 6 | Offseason flow is sequential; players cannot skip or reorder steps. | Each step depends on prior step outputs (contracts before pre-season lock). |
| 7 | HQ tab switching is instant and must not show loading screens. | Supports Pillar 7 and enforces low-friction navigation. |
| 8 | Max 3 clicks from HQ to queue any action. | Hard usability rule from Pillar 7. |
| 9 | Only critical inbox messages may block progression with mandatory acknowledgment. | Prevents non-essential interruption while preserving safety blockers. |
| 10 | Weekend back-navigation is review-only for completed sessions; no session reruns. | Preserves fairness and deterministic event history. |

Non-negotiable MVP release rule:
- ALPHA tabs are hidden in MVP release (not disabled or greyed).

## 8. Data Dependency Map

Section 8 is the UI->App data contract. App layer must expose these data groups to screen consumers.

| Data source | Screens that read it |
|---|---|
| Team state (budget, name, roster) | Home, Drivers, Car, Staff, Finances, all Weekend screens |
| Driver entities (stats, contracts, morale) | Drivers, Home, Live Race, Qualifying, Results |
| Part entities (performance, wear, ranking) | Car, Home, Practice, Live Race |
| Staff entities (skills, contracts) | Staff |
| Sponsor contracts and offers | Sponsors, Inbox, Finances |
| Facility state (levels, upgrades) | Facilities |
| Season calendar | Calendar, Home, Pre-Race Overview |
| Championship standings | Home, Calendar, Results |
| Inbox messages | Inbox, Home (badge count) |
| Regulation state | Politics |
| Circuit definitions | Pre-Race Overview, Practice, Qualifying, Live Race |
| Weather state | Pre-Race Overview, Practice, Qualifying, Live Race |
| Race simulation state (positions, times) | Live Race, Results |
| Setup presets | Car, Practice, Pre-Race Strategy |
| Build queues (parts, facilities) | Car, Facilities |
| Save slot metadata | Save/Load |
| Player preferences | Settings |

## 9. Invariants

| Invariant | Rule | What breaks if violated |
|---|---|---|
| Only one mode active | HQ, Weekend, and Offseason are mutually exclusive game-shell states. | UI can render conflicting contexts and invalid controls simultaneously. |
| Tab count fixed per scope | MVP release uses 7-tab navigation model (6 visible tabs plus Advance Week footer control). ALPHA adds Staff, Sponsors, Facilities, and Politics. | Navigation layout contract breaks and click-budget guarantees fail. |
| Weekend cannot be exited early | No path from weekend to HQ before results acknowledgment. | Mid-weekend state can be orphaned, breaking save/load integrity. |
| Save captures exact screen state | Save at a race-lap boundary must restore identical boundary state. | Player progress and deterministic continuation are lost. |
| Screen data is read-only except through actions | Screens consume App view data; all mutations flow through documented action pathways. | Determinism and traceability fail due to implicit UI-side mutation. |

## 10. Verification Strategy

| Test | Input | Expected | What it proves |
|---|---|---|---|
| Click count audit | Navigate from Home to each action path in HQ. | No action path exceeds 3 clicks from Home. | Pillar 7 navigation budget compliance. |
| Tab switch speed | Cycle through all visible HQ tabs repeatedly. | Under 100ms switch and no loading indicator. | Instant-tab rule compliance. |
| Weekend modal enforcement | During weekend, attempt HQ tab access. | Access blocked with weekend-in-progress feedback. | Modal integrity enforcement. |
| Save during race | Save between lap 35 and lap 36, then load. | Restores exact post-lap-35 state and resumes at lap 36 boundary. | Save-anytime plus deterministic resume behavior. |
| Setup preset persistence | Create preset at Race 1 and check at Race 5. | Preset remains available and usable. | Cross-weekend setup persistence. |
| MVP tab visibility | Launch MVP release build. | Only MVP tab set visible; ALPHA tabs hidden. | Scope-gate visibility rule enforcement. |
| Data availability | Open each screen once in valid progression state. | Screen renders without missing-data errors. | Data dependency contract completeness. |
| Offseason sequence | Complete final race and enter offseason. | Offseason screens appear in required sequence with no skip. | Sequential offseason flow enforcement. |
| Weekend back-navigation | After qualifying, navigate back to practice. | Shows read-only historical output; no rerun action available. | Review-only back-navigation enforcement. |
