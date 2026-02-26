# Game Pillars

> Status: LOCKED
> Approved: February 26, 2026
> Owner: Ayub Ashraf
>
> All design decisions must reference at least one pillar.
> To change a pillar requires unanimous team agreement and an ADR.

## How to Use This Document

- Before designing a feature, identify which pillars it serves.
- If a feature conflicts with a pillar, the pillar wins unless an ADR overrides it.
- During code review, reviewers may reject changes that violate a pillar.

## The Seven Pillars

### Pillar 1 — Deterministic Simulation

#### Statement

Given the same seed and the same player decisions, the simulation must produce identical outcomes on every run and on every platform.

#### Why it matters

Players must trust that strategic choices determine outcomes. Reproducible runs also enable reliable replay analysis and defect isolation.

#### What this means in practice

- The simulation must use a single seeded random source.
- Time, money, and rates in simulation calculations must use fixed-precision numeric representations.
- Simulation state must be serializable and restorable between any two steps.
- Tie-break behavior must be defined explicitly and must never depend on non-deterministic container ordering.

#### Violation examples

- Using wall-clock time as simulation input.
- Calling engine-level random APIs from deterministic simulation logic.
- Relying on unordered collection iteration as a tie-break rule.
- Using non-deterministic numeric behavior in lap outcome calculations.

#### Test it by

Run the same 20-car, 70-lap race 1000 times with seed 42 and identical decisions. The results must be byte-identical across all runs.

### Pillar 2 — Data-Driven Balance

#### Statement

Every balance and tuning value must live in external data definitions, and gameplay rules must never rely on magic numbers embedded in logic.

#### Why it matters

Balance must be tunable without refactoring logic. Data-driven tuning reduces regression risk and allows controlled experimentation.

#### What this means in practice

- Degradation curves, lap modifiers, cost tables, and reliability rates must be stored in definition assets.
- Runtime logic must read tuning values from definitions and must never hardcode thresholds or coefficients.
- Definition packs must be replaceable without changing gameplay logic.
- Validation must reject malformed or incomplete definition data before a season starts.

#### Violation examples

- Hardcoding a strategy threshold directly in gameplay logic.
- Embedding pit-stop duration constants directly in orchestration logic.
- Storing a balance coefficient as a compile-time constant instead of external data.

#### Test it by

Replace the default definition pack with a test pack. The game must load, simulate, and complete a season without errors.

### Pillar 3 — Meaningful Tradeoffs

#### Statement

Every player decision must carry a real benefit and a real cost, and no choice may be strictly dominant across all conditions.

#### Why it matters

Strategic gameplay depends on meaningful tension between options. Dominant choices collapse decision quality and reduce replay value.

#### What this means in practice

- Setup choices must improve one performance axis while reducing another.
- Strategy choices must provide short-term gains that introduce long-term risk.
- Budget choices must force quality-versus-cost tradeoffs.
- Each major system must expose at least two competing decision axes.

#### Violation examples

- A part upgrade that is better in all metrics with no downside.
- A strategy that consistently wins regardless of track characteristics.
- A driver attribute that has no measurable impact on simulation outcomes.

#### Test it by

Simulate 100 seasons with random-but-valid AI decisions. No single strategy may win more than 60% of championships, and no single part choice may exceed 90% selection rate.

### Pillar 4 — Readable Numbers

#### Statement

Every modifier that affects an outcome must be inspectable by the player.

#### Why it matters

Players make better decisions when outcomes are explainable. Transparent systems create trust and support deliberate strategy.

#### What this means in practice

- Outcome screens must present ordered modifier breakdowns for key results.
- Lap outcomes must expose contributions such as base pace, driver delta, tire state, fuel state, and weather effects.
- Finance outcomes must expose both income sources and cost lines.
- Progression outcomes must expose all contributors to development speed.

#### Violation examples

- Applying a hidden modifier that players cannot inspect.
- Showing qualitative claims about performance without numeric evidence.
- Producing lap outcomes that cannot be decomposed into component modifiers.

#### Test it by

For any simulation output, the UI must display the ordered list of modifiers that produced it. A lap-time breakdown test must confirm all modifiers are present and sum to the final value.

### Pillar 5 — Strategic Depth

#### Statement

No single dominant strategy may remain optimal across all situations.

#### Why it matters

Long-term engagement requires changing optimal play as conditions evolve. Strategy depth comes from adaptation, not repetition.

#### What this means in practice

- Circuit characteristics must change which setup and strategy profiles are optimal.
- Weather and conditions must shift relative strategy value.
- Regulation changes must periodically reset or reshape the competitive meta.
- Opponents must adapt over time to repeated player patterns.

#### Violation examples

- One tire plan remains optimal on every circuit profile.
- One development path always produces the fastest long-term outcomes.
- Opponents repeat the same approach over seasons without adaptation.

#### Test it by

Across 10 circuit profiles, optimal pit strategy must vary. Across regulation changes, the championship leader must change at least once in 50 simulated seasons.

### Pillar 6 — Abstract Presentation

#### Statement

The game must remain a tick-based management simulation and must never become a real-time 3D racing experience.

#### Why it matters

The project focus is strategic management clarity, not vehicle rendering fidelity. Preserving abstraction protects scope and development velocity.

#### What this means in practice

- Race progression must run in logical simulation steps.
- Presentation must focus on positions, gaps, events, tables, and charts.
- Simulation correctness must not depend on rendering or frame cadence.
- Race simulation must execute headless without scene object dependencies.

#### Violation examples

- Adding a track view that requires rendered car models for race logic.
- Introducing real-time physics as a race outcome dependency.
- Binding simulation progress to frame rate or visual update loops.

#### Test it by

Run race simulation headless in an EditMode test with no active scene objects. Results and performance must remain valid independent of frame rate.

### Pillar 7 — Respect for Player Time

#### Statement

The interface must minimize friction so player time goes to decisions, not navigation overhead.

#### Why it matters

Management depth only works when interaction cost stays low. Fast access and fast progression preserve decision-focused gameplay.

#### What this means in practice

- Advancing core calendar flow must require minimal actions.
- Simulation speed must be player-controlled up to near-instant resolution.
- Key decision data must be visible without deep menu drilling.
- Progression must autosave after each race weekend.
- Non-decision-blocking delays and unskippable transitions must never gate progress.

#### Violation examples

- Requiring more than three interactions to reach key management screens from Team HQ.
- Imposing mandatory wait time during simulation when fast-forward is available.
- Blocking progression with modal prompts that require no real choice.
- Requiring manual menu-only saves for core progress safety.

#### Test it by

Count interactions from Team HQ to every leaf screen; none may exceed three. Measure time from "advance week" to "ready for next decision" at maximum speed; it must be under two seconds.
