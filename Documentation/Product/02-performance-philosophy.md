# Performance Philosophy — Lap Time Model

> Status: LOCKED
> Approved: February 26, 2026
> Depends on: Product/01-game-pillars.md (Pillars 1, 2, 3, 4)
>
> This document defines how every lap time in the game is calculated.
> All simulation code must implement this formula exactly.
> Changing the formula requires an ADR.

## 1. The Core Formula

```text
LapTime =
    TrackBaseTime
  + Σ PartRankingTimeCosts
  + DriverPaceModifier
  + SetupModifier
  + KnowledgeModifier
  + TyrePhaseModifier
  + TyreTemperatureModifier
  + FuelWeightModifier
  + EngineModeModifier
  + WeatherModifier
  + DraftingModifier
  + RandomVariance(seed, lapIndex)
```

Rules:
- All values must use integer milliseconds only; floating-point values are never allowed.
- All modifiers must be additive; final lap time must equal the exact sum of all terms.
- The formula must be identical for AI and player teams; hidden bonuses and rubber-banding are never allowed.
- Every modifier must be tunable through external scalar files.
- Every modifier must be inspectable in a player-visible lap-time breakdown.

Numeric labeling rule for this document:
- Every numeric value below is an explicitly labeled scalar-defined illustrative value from external files.

## 2. The Ranking Principle

Statement:
- Parts must contribute rank position, not direct raw performance time.
- Rank position must map to lap-time cost using scalar-defined lookup tables.

Worked example (all time costs are scalar-defined illustrative values from rank scalar files):

| Team | Engine Performance (scalar-defined illustrative stat) | Engine Rank | Time Cost (scalar-defined illustrative value from scalar file) |
|---|---:|---:|---:|
| Team A | 92 | 1 | +0ms |
| Team B | 88 | 2 | +80ms |
| Team C | 85 | 3 | +160ms |
| Team D | 82 | 4 | +240ms |
| Team E | 79 | 5 | +320ms |
| Team F | 76 | 6 | +400ms |
| Team G | 72 | 7 | +480ms |
| Team H | 68 | 8 | +560ms |
| Team I | 64 | 9 | +640ms |
| Team J | 61 | 10 | +720ms |

Required rules:
- Ranking must matter more than raw stat spacing. If two teams remain in the same rank, both must keep the same rank-based time cost.
- Rankings must be computed per part category at minimum for Engine, Aero, and Chassis, and each category must have its own scalar-defined rank table.
- Rank time costs must be sourced from external scalar files so one file change can rebalance the whole field.
- Tied rankings must use standard competition ranking (`1, 1, 3`).

Design rationale:
- The Motorsport Manager-inspired behavior must prioritize competitive position over absolute stat magnitude, which keeps field balance stable even when raw stat ranges are wide.

## 3. Modifier Catalog

### 3.1 TrackBaseTime

- Name: `TrackBaseTime`
- Purpose: Represents the idealized perfect lap baseline for the circuit.
- Input: Circuit definition attributes (track length, corner profile, straight profile, elevation profile).
- Output: Base lap time in milliseconds before dynamic modifiers.
- Scalar file: Circuit definition file per track (example path shape: `circuits/<track>.json`).
- Range: Scalar-defined illustrative range from external files, for example `60000ms` to `140000ms`.
- Update frequency: Fixed by circuit definition; never changes during a season unless definitions are changed.
- Player visibility: Must appear as `Track Base` in lap breakdown.

### 3.2 PartRankingTimeCosts

- Name: `PartRankingTimeCosts`
- Purpose: Converts team part rankings into additive time costs across categories.
- Input: Team part performance values, field-wide category ranking, and per-category rank lookup values.
- Output: Sum of category rank costs in milliseconds.
- Scalar file: `part_rank_time_cost.json`.
- Range: Scalar-defined illustrative range from external files, for example `0ms` to `2500ms` depending on field size and table values.
- Update frequency: Recomputed whenever any team part performance changes (development updates and regulation resets).
- Player visibility: Must appear as category lines, for example `Engine Rank: 3rd (+160ms, scalar-defined illustrative value)`.

Additional required rules:
- Tie handling must use standard competition ranking (`1, 1, 3`).
- Mid-season development must immediately affect rank outcomes through the same rank-table process.

### 3.3 DriverPaceModifier

- Name: `DriverPaceModifier`
- Purpose: Captures driver pace quality relative to baseline talent.
- Input: Driver speed rating, driver consistency rating, and wet-related driver capability in wet conditions.
- Output: Millisecond delta versus a baseline driver.
- Scalar file: `driver_pace_scalars.json`.
- Range: Scalar-defined illustrative range from external files, for example `-400ms` to `+400ms`.
- Update frequency: Pace baseline applies per session; variance contribution can affect each lap through seeded randomness.
- Player visibility: Must appear as a line such as `Driver Skill: -220ms (scalar-defined illustrative value)`.

Additional required rules:
- Consistency must control spread width, not mean pace.
- Mean pace must come from speed rating, while variance width must come from consistency rating.
- Variance must be deterministic from race seed and lap progression.

### 3.4 SetupModifier

- Name: `SetupModifier`
- Purpose: Represents lap-time penalty from mismatch between car setup and circuit needs.
- Input: Setup configuration and circuit ideal setup profile mapped to setup match percentage.
- Output: Positive millisecond penalty for setup mismatch.
- Scalar file: `setup_scalars.json`.
- Range: Scalar-defined illustrative range from external files, for example `0ms` to `+600ms`.
- Update frequency: Tuned during practice, then locked by race setup rules.
- Player visibility: Must appear as `Setup Match: <pct> (+<ms>)` with scalar-defined illustrative values.

Additional required rules:
- Setup must remain a qualifying-versus-race tradeoff.
- Practice activity must reduce setup uncertainty.

### 3.5 KnowledgeModifier

- Name: `KnowledgeModifier`
- Purpose: Models lap-time effect from accumulated track knowledge.
- Input: Practice-earned knowledge level for the circuit and retained knowledge from prior seasons.
- Output: Millisecond penalty that shrinks as knowledge increases.
- Scalar file: `knowledge_scalars.json`.
- Range: Scalar-defined illustrative range from external files, for example `0ms` to `+300ms`.
- Update frequency: Improves during practice sessions and locks once qualifying begins.
- Player visibility: Must appear as `Track Knowledge: <pct> (+<ms>)` with scalar-defined illustrative values.

Additional required rules:
- First visit to a circuit must start from scalar-defined baseline knowledge (illustrative baseline example: `0%`).
- Returning seasons must apply scalar-defined retention.
- Knowledge must cap setup optimization potential until sufficient knowledge is reached.

### 3.6 TyrePhaseModifier

- Name: `TyrePhaseModifier`
- Purpose: Models performance evolution across tire life phases.
- Input: Compound definition, laps since stop, and phase thresholds per compound.
- Output: Millisecond delta by tire life phase.
- Scalar file: `tyre_compound_defs.json`.
- Range: Scalar-defined illustrative range from external files, for example `-200ms` to `+2000ms`.
- Update frequency: Recomputed every lap.
- Player visibility: Must appear as phase-specific labels such as `Tyre Phase: Optimal (-80ms, scalar-defined illustrative value)`.

Additional required rules:
- Four phases must exist: Fresh, Optimal, Worn, and Cliff.
- Phase boundaries must come from compound scalar definitions.
- Softer compounds must reach peak sooner and cliff sooner via scalar-defined phase thresholds.
- Harder compounds must warm slower and cliff later via scalar-defined phase thresholds.

### 3.7 TyreTemperatureModifier

- Name: `TyreTemperatureModifier`
- Purpose: Applies penalty when tire temperature is outside optimal operating window.
- Input: Current tire temperature, compound temperature window, ambient/track temperature, and driving intensity.
- Output: Positive millisecond penalty for temperature mismatch.
- Scalar file: `tyre_temperature_scalars.json`.
- Range: Scalar-defined illustrative range from external files, for example `0ms` to `+500ms`.
- Update frequency: Recomputed every lap.
- Player visibility: Must appear as state labels such as `Tyre Temp: Cold (+280ms, scalar-defined illustrative value)`.

Additional required rules:
- Colder weather must cool tires faster through scalar-defined thermal effects.
- Hotter weather must heat tires faster through scalar-defined thermal effects.
- Softer compounds must warm faster and overheat sooner according to scalar definitions.
- Harder compounds must warm slower according to scalar definitions.

### 3.8 FuelWeightModifier

- Name: `FuelWeightModifier`
- Purpose: Converts carried fuel mass to lap-time penalty.
- Input: Current fuel load and circuit-defined consumption behavior.
- Output: Positive millisecond penalty from fuel weight.
- Scalar file: `fuel_scalars.json`.
- Range: Scalar-defined illustrative range from external files, for example `0ms` to `+1200ms`.
- Update frequency: Recomputed every lap as fuel decreases.
- Player visibility: Must appear as `Fuel Load: <kg> (+<ms>)` with scalar-defined illustrative values.

Additional required rules:
- Relationship must be linear based on scalar-defined conversion.
- Fuel burn per lap must come from external definitions.
- Fuel depletion must produce retirement behavior under race rules.

### 3.9 EngineModeModifier

- Name: `EngineModeModifier`
- Purpose: Applies pace-vs-reliability tradeoff from selected engine mode.
- Input: Active engine mode selection.
- Output: Millisecond delta from selected mode profile.
- Scalar file: `engine_mode_defs.json`.
- Range: Scalar-defined illustrative range from external files, for example `-300ms` to `+200ms`.
- Update frequency: Adjustable by strategy decisions at lap or stint intervals under race rules.
- Player visibility: Must appear as mode label such as `Engine Mode: Push (-180ms, scalar-defined illustrative value)`.

Additional required rules:
- Aggressive modes must increase failure risk through scalar-defined reliability multipliers.
- Conservation modes must reduce risk and improve fuel efficiency through scalar-defined effects.

### 3.10 WeatherModifier

- Name: `WeatherModifier`
- Purpose: Models weather grip loss and tire-compound suitability mismatch.
- Input: Current weather state and compound suitability matrix.
- Output: Additive millisecond penalty combining base weather effect and compound mismatch effect.
- Scalar file: `weather_scalars.json`.
- Range: Scalar-defined illustrative range from external files, for example `0ms` to `+8000ms`.
- Update frequency: Recomputed every lap as weather evolves.
- Player visibility: Must show both weather penalty and compound-match penalty lines with scalar-defined illustrative values.

Additional required rules:
- Modifier must have two components: base grip reduction and compound suitability.
- Intermediate compounds in damp conditions must carry smaller scalar-defined penalties than dry compounds in wet conditions.
- Wet compounds in dry conditions must carry scalar-defined overheating penalties.
- Crossover decision timing must remain a major strategy lever.

### 3.11 DraftingModifier

- Name: `DraftingModifier`
- Purpose: Represents slipstream and DRS-related pace benefit from proximity to the car ahead.
- Input: Gap to immediately preceding car, DRS eligibility state, and circuit DRS availability rules.
- Output: Negative millisecond benefit when conditions are satisfied.
- Scalar file: `drafting_scalars.json`.
- Range: Scalar-defined illustrative range from external files, for example `0ms` to `-300ms`.
- Update frequency: Recomputed every lap.
- Player visibility: Must appear as a line such as `Drafting: -120ms (scalar-defined illustrative value)`.

Additional required rules:
- Drafting benefit must diminish with larger gap according to scalar-defined thresholds.
- No-benefit threshold must come from scalar file definitions.
- DRS bonus must come from scalar file definitions and only apply in valid conditions.
- DRS must remain disabled in wet conditions and for the first scalar-defined illustrative restart window (illustrative policy value: first `2` laps, defined in rules data).

### 3.12 RandomVariance

- Name: `RandomVariance`
- Purpose: Produces controlled lap-to-lap variation while preserving deterministic replay.
- Input: Race seed, lap index progression, and driver consistency-based attenuation.
- Output: Small signed millisecond delta.
- Scalar file: `variance_scalars.json`.
- Range: Scalar-defined illustrative range from external files, for example `-150ms` to `+150ms` before consistency attenuation.
- Update frequency: Recomputed every lap from deterministic seeded sequence.
- Player visibility: Hidden from default summary; may appear in detailed diagnostics as scalar-defined illustrative variance context.

Additional required rules:
- Variance must be generated from a deterministic seeded RNG sequence.
- Consistency must attenuate variance width; higher consistency must yield tighter spread.
- Race seed must be set once and must remain constant for the event.

## 4. Scalar File Contracts

Required files and illustrative schema shapes (all numeric values are scalar-defined illustrative examples):

| File | Contents | Schema shape (illustrative) |
|---|---|---|
| `part_rank_time_cost.json` | Rank-to-milliseconds per part category | `{ "engine": [0, 80, 160], "aero": [0, 70, 140], "chassis": [0, 60, 120] }` |
| `driver_pace_scalars.json` | Rating-to-time conversion | `{ "baseline_rating": 70, "ms_per_rating_point": 8 }` |
| `setup_scalars.json` | Setup match percentage to penalty | `{ "penalties": [{ "match_pct": 100, "ms": 0 }, { "match_pct": 0, "ms": 600 }] }` |
| `knowledge_scalars.json` | Knowledge percentage effects | `{ "max_penalty_ms": 300, "retention_pct_per_season": 40 }` |
| `tyre_compound_defs.json` | Compound phase thresholds and degradation behavior | `{ "soft": { "fresh_laps": 3, "optimal_laps": 8, "worn_laps": 5, "cliff_ms_per_lap": 400 } }` |
| `tyre_temperature_scalars.json` | Temperature delta to penalty | `{ "ms_per_degree_outside_window": 25 }` |
| `fuel_scalars.json` | Fuel mass to time penalty | `{ "ms_per_kg": 8 }` |
| `engine_mode_defs.json` | Mode pace and reliability multipliers | `{ "push": { "ms": -180, "reliability_mult": 1.5 }, "conserve": { "ms": 100, "reliability_mult": 0.7 } }` |
| `weather_scalars.json` | Weather base penalty and compound suitability matrix | `{ "wet": { "base_penalty_ms": 3000, "compound_match": { "wet": 0, "inter": 800, "soft": 6000 } } }` |
| `drafting_scalars.json` | Gap-to-benefit and DRS bonus | `{ "max_benefit_ms": 300, "threshold_gap_ms": 2000, "drs_bonus_ms": 100 }` |
| `variance_scalars.json` | Base variance and consistency attenuation | `{ "base_range_ms": 150, "consistency_attenuation": 0.5 }` |

Contract statements:
- Schemas are illustrative.
- Exact field names and nesting must be finalized in Phase 1 domain model implementation.
- Every number in the lap-time formula must come from one of these external files.

## 5. Invariants & Bounds

| Invariant | Rule | What breaks if violated | Protected Pillars |
|---|---|---|---|
| Lap time is always positive | Lap time must always be greater than zero. | Negative lap times corrupt ordering and standings logic. | Pillar 1, Pillar 3 |
| Lap time uses integer milliseconds | All intermediate and final values must remain integer milliseconds. | Cross-platform determinism and reproducibility degrade. | Pillar 1 |
| All modifiers are bounded | Every modifier must have scalar-defined min and max limits. | Edge cases can produce implausible or unstable outcomes. | Pillar 2, Pillar 3 |
| Ranking domain is valid | Rank inputs must remain in range `1..N` where `N` is field size from external definitions. | Out-of-range access can invalidate rank lookup behavior. | Pillar 1, Pillar 2 |
| Same inputs yield same output | Given identical state and seed, lap calculation must be pure and deterministic. | Determinism guarantee fails and replay consistency breaks. | Pillar 1 |
| Formula symmetry for AI and player | AI and player teams must use the exact same modifier model. | Competitive fairness breaks due to hidden asymmetry. | Pillar 3, Pillar 4 |
| All scalars are externalized | Literal tuning numbers must never be hardcoded in lap-time logic. | Data-driven tuning and rebalance workflow break. | Pillar 2 |
| Each modifier is decomposable | Breakdown output must sum exactly to total lap time. | Player cannot inspect causes of performance changes. | Pillar 4 |
| Modifier order is computation-invariant | Because terms are additive, arithmetic order must not change final value. | Inconsistent reporting and validation confusion can occur. | Pillar 1, Pillar 4 |

## 6. Verification Strategy

All test values below are scalar-defined illustrative examples.

| Test | Input | Expected | Pillar verified |
|---|---|---|---|
| Determinism | Same race state, seed `42`, lap index `15`, repeated `1000` times. | Identical millisecond output every run. | Pillar 1 |
| Scalar swap | Replace `part_rank_time_cost.json` with doubled scalar-defined illustrative values. | Competitive outcomes shift materially under new scalar set. | Pillar 2 |
| Breakdown sum | Request modifier breakdown for any lap. | Sum of modifier terms equals total lap time exactly. | Pillar 4 |
| No magic numbers | Static scan of lap-time calculation paths for hardcoded tuning literals. | No hardcoded tuning literals in lap-time model paths. | Pillar 2 |
| AI parity | Compare modifier application for AI and player scenarios with identical inputs. | Identical formula and no role-based branching in behavior. | Pillar 3 |
| Bounds | Stress scenarios with scalar-defined illustrative extremes (for example rank `1` and rank `40`, fuel `0kg` and `200kg`, monsoon plus dry compound). | Lap time stays positive and within scalar-defined illustrative plausibility envelope (for example `60000ms` to `300000ms`). | Pillar 1, Pillar 2 |
| Rank ties | Two teams share identical category performance values. | Both receive the same rank and the next rank is skipped (`1, 1, 3`). | Pillar 1, Pillar 3 |
| Zero knowledge | Driver starts at scalar-defined illustrative baseline knowledge (`0%`). | Knowledge penalty equals configured maximum from scalar file. | Pillar 2, Pillar 4 |
| Full cliff | Soft compound exceeds scalar-defined illustrative cliff threshold (for example lap `30` when cliff threshold is scalar-defined illustrative lap `16`). | Tyre phase reports cliff and lap time shows severe degradation. | Pillar 2, Pillar 4 |
| Weather crossover | Condition transition from dry to wet at scalar-defined illustrative lap `20` while still on dry compound. | Weather and compound mismatch penalties jump to large scalar-defined values and lap time slows by multiple seconds. | Pillar 3, Pillar 4 |
