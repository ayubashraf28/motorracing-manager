# Definition Types — Immutable Data Architecture

> Status: LOCKED
> Approved: March 1, 2026
> Depends on: Product/01-game-pillars.md (Pillars 2, 6), Product/02-performance-philosophy.md (Section 4)
>
> This document defines every immutable definition type in the game.
> All definition types must be implemented exactly as specified.
> Adding or changing a definition type requires an ADR.

## 1. The Definition/Runtime Principle

Canonical rule:

- Definitions are loaded from external data files at startup.
- Definitions are never mutated during gameplay.
- Runtime state such as teams, drivers, contracts, and finances is mutable and saved.
- Swapping a complete definition pack must produce a valid, playable game with no code changes.

Definition versus runtime state:

| Definition (immutable, loaded) | Runtime (mutable, saved) |
|---|---|
| Track layouts, base times, sectors | Team budget, roster, standings |
| Tyre compound specs | Tyre wear on a specific car this lap |
| Series calendar template | Current round, completed races |
| Ruleset (points, regulations) | Championship points accumulated |
| Part type specs (stat ranges) | Specific part instance with wear |
| Sponsor templates | Active sponsor contract with team |
| Building specs (levels, costs) | Building upgrade progress |
| Driver archetypes (stat ranges) | Specific driver with current ratings |
| Scalar tables (balance values) | — (consumed by sim, not saved) |
| Weather types and transitions | Current weather state in a race |

## 2. Identifier Types

All identifiers are string-backed, strongly typed value wrappers.

| Identifier | Wraps | Example Values | Layer |
|---|---|---|---|
| `SeriesId` | `string` | `"f1"`, `"indycar"`, `"f2"` | Core |
| `TrackId` | `string` | `"monaco"`, `"silverstone"`, `"monza"` | Core |
| `RulesetId` | `string` | `"f1_2030_rules"` | Core |
| `PartTypeId` | `string` | `"engine_v1"`, `"aero_front"` | Core |
| `TyreCompoundId` | `string` | `"soft"`, `"medium"`, `"hard"`, `"inter"`, `"wet"` | Core |
| `SponsorId` | `string` | `"megacorp"`, `"fuel_co"` | Core |
| `BuildingId` | `string` | `"wind_tunnel"`, `"factory"`, `"simulator"` | Core |
| `ComponentId` | `string` | `"turbo_a"`, `"wing_element_b"` | Core |
| `DriverArchetypeId` | `string` | `"veteran"`, `"rookie_talent"` | Core |
| `WeatherTypeId` | `string` | `"dry"`, `"damp"`, `"wet"`, `"monsoon"` | Core |
| `EngineModeId` | `string` | `"push"`, `"standard"`, `"conserve"` | Core |

Rules:

- All identifiers are `readonly struct` implementing `IEquatable<T>`.
- The constructor rejects null, empty, and whitespace-only values.
- Equality uses `StringComparer.Ordinal`.
- Identifiers are string-based for human-readable data files and mod-readiness.

## 3. Supporting Enums

| Enum | Values | Layer |
|---|---|---|
| `PartSlot` | `Engine`, `Aero`, `Chassis` | Core |
| `SponsorTier` | `Title`, `Major`, `Minor`, `Technical` | Core |
| `WeatherState` | `Dry`, `Damp`, `Wet`, `Monsoon` | Core |
| `TyrePhase` | `Fresh`, `Optimal`, `Worn`, `Cliff` | Core |
| `SessionType` | `Practice1`, `Practice2`, `Practice3`, `Qualifying1`, `Qualifying2`, `Qualifying3`, `SprintQualifying`, `Sprint`, `Race` | Core |
| `QualifyingFormat` | `SingleSession`, `ThreeKnockout` | Core |
| `BuildingEffectType` | `DevelopmentSpeed`, `ReliabilityBonus`, `PitStopSpeed`, `SetupAccuracy`, `KnowledgeRetention` | Core |

## 4. Definition Type Catalog

Mandatory numeric conventions across all definition types:

- Time uses integer milliseconds and the field suffix `Ms`.
- Money uses integer cents and the field suffix `Cents`. Use `long` where values exceed `int` range.
- Ratios and ratings use integer basis points `0..10000` and the field suffix `BasisPoints`, where `10000 = 100% = 1.0x`, unless a field explicitly documents that it may exceed `10000`.
- Temperature uses integer Celsius and the field suffix `Celsius`.
- No `float` or `double` fields are allowed in any definition type.

### 4.1 `TyreCompoundDef` (Core)

Purpose: Immutable definition of tyre compound behavior across grip, wear, and temperature windows.

| Field | Type | Unit / Convention | Constraints |
|---|---|---|---|
| `Id` | `TyreCompoundId` | Identifier | Unique across pack |
| `DisplayName` | `string` | Text | Non-empty |
| `GripRatingBasisPoints` | `int` | Basis points | `0..10000` |
| `WearRatePerLapBasisPoints` | `int` | Basis points per lap | `> 0` |
| `FreshLaps` | `int` | Laps | `>= 0` |
| `OptimalLaps` | `int` | Laps | `> 0` |
| `WornLaps` | `int` | Laps | `>= 0` |
| `CliffMsPerLap` | `int` | Milliseconds per lap | `> 0` |
| `OptimalBonusMs` | `int` | Milliseconds | Negative value |
| `FreshPenaltyMs` | `int` | Milliseconds | Positive value |
| `TemperatureWindowLowCelsius` | `int` | Celsius | `< TemperatureWindowHighCelsius` |
| `TemperatureWindowHighCelsius` | `int` | Celsius | `> TemperatureWindowLowCelsius` |
| `WarmupRateCelsiusPerLap` | `int` | Celsius per lap | `> 0` |

Illustrative JSON:

```json
{
  "id": "soft",
  "displayName": "Soft",
  "gripRatingBasisPoints": 9000,
  "wearRatePerLapBasisPoints": 300,
  "freshLaps": 2,
  "optimalLaps": 6,
  "wornLaps": 4,
  "cliffMsPerLap": 400,
  "optimalBonusMs": -80,
  "freshPenaltyMs": 200,
  "temperatureWindowLowCelsius": 85,
  "temperatureWindowHighCelsius": 110,
  "warmupRateCelsiusPerLap": 8
}
```

### 4.2 `WeatherDef` (Core)

Purpose: Immutable definition of a weather type, its grip effect, and transition behavior.

| Field | Type | Unit / Convention | Constraints |
|---|---|---|---|
| `Id` | `WeatherTypeId` | Identifier | Unique |
| `DisplayName` | `string` | Text | Non-empty |
| `State` | `WeatherState` | Enum | Valid enum |
| `BasePenaltyMs` | `int` | Milliseconds | `>= 0` |
| `GripModifierBasisPoints` | `int` | Basis points | `0..10000` |
| `CompoundMatchPenaltyMs` | `IReadOnlyDictionary<TyreCompoundId, int>` | Milliseconds by compound | All referenced compounds must exist in pack |
| `TransitionProbabilityBasisPoints` | `IReadOnlyDictionary<WeatherTypeId, int>` | Basis points by weather type | Values sum to `10000` |
| `TrackTemperatureDeltaCelsius` | `int` | Celsius delta | Offset from ambient |

Illustrative JSON:

```json
{
  "id": "wet",
  "displayName": "Wet",
  "state": "Wet",
  "basePenaltyMs": 3000,
  "gripModifierBasisPoints": 4000,
  "compoundMatchPenaltyMs": {
    "wet": 0,
    "inter": 800,
    "soft": 6000,
    "medium": 5500,
    "hard": 5000
  },
  "transitionProbabilityBasisPoints": {
    "dry": 500,
    "damp": 2000,
    "wet": 6500,
    "monsoon": 1000
  },
  "trackTemperatureDeltaCelsius": -8
}
```

### 4.3 `ScalarTables` (Core)

Purpose: Immutable facade aggregating all scalar sub-tables consumed by deterministic simulation.

| Sub-table | Fields | Source File |
|---|---|---|
| `PartRankTimeCostTable` | `RankToMsByCategory: IReadOnlyDictionary<PartSlot, IReadOnlyList<int>>` | `part_rank_time_cost.json` |
| `DriverPaceScalars` | `BaselineRating: int`, `MsPerRatingPoint: int` | `driver_pace_scalars.json` |
| `SetupScalars` | `Penalties: IReadOnlyList<SetupPenaltyEntry>` | `setup_scalars.json` |
| `KnowledgeScalars` | `MaxPenaltyMs: int`, `RetentionPercentPerSeason: int` | `knowledge_scalars.json` |
| `TyreTemperatureScalars` | `MsPerDegreeOutsideWindow: int` | `tyre_temperature_scalars.json` |
| `FuelScalars` | `MsPerKg: int` | `fuel_scalars.json` |
| `EngineModes` | `IReadOnlyList<EngineModeScalar>` | `engine_mode_defs.json` |
| `DraftingScalars` | `MaxBenefitMs: int`, `ThresholdGapMs: int`, `DrsBonusMs: int` | `drafting_scalars.json` |
| `VarianceScalars` | `BaseRangeMs: int`, `ConsistencyAttenuationBasisPoints: int` | `variance_scalars.json` |

Supporting value types:

- `SetupPenaltyEntry` (`readonly struct`): `MatchPercentage`, `PenaltyMs`
- `EngineModeScalar` (`sealed class`): `Id`, `PaceModifierMs`, `ReliabilityMultiplierBasisPoints`, `FuelEfficiencyMultiplierBasisPoints`

Important conversions from Performance Philosophy:

- `"consistency_attenuation": 0.5` becomes `ConsistencyAttenuationBasisPoints: 5000`
- `"reliability_mult": 1.5` becomes `ReliabilityMultiplierBasisPoints: 15000`

`PartRankTimeCostTable` exposes `LookupMs(PartSlot slot, int rank)` with these rules:

- Returns the millisecond cost for the given rank position.
- Clamps out-of-range ranks: rank `0` or less returns the first entry, and rank greater than the table length returns the last entry.
- Rank tables must be monotonically non-decreasing.

### 4.4 `SeriesDef` (Domain)

Purpose: Immutable definition of a racing series and its high-level season structure.

| Field | Type | Unit / Convention | Constraints |
|---|---|---|---|
| `Id` | `SeriesId` | Identifier | Unique |
| `DisplayName` | `string` | Text | Non-empty |
| `Tier` | `int` | Ordinal | `>= 1`, where `1` is top tier |
| `RulesetId` | `RulesetId` | Identifier | Must exist in pack |
| `CalendarTemplate` | `IReadOnlyList<TrackId>` | Ordered track ids | Non-empty, all referenced tracks exist |
| `TeamCount` | `int` | Teams | `> 0` |
| `DriversPerTeam` | `int` | Drivers | `> 0` |

Illustrative JSON:

```json
{
  "id": "f1",
  "displayName": "Formula 1 Championship",
  "tier": 1,
  "rulesetId": "f1_2030_rules",
  "calendarTemplate": ["bahrain", "jeddah", "melbourne", "monaco", "silverstone"],
  "teamCount": 10,
  "driversPerTeam": 2
}
```

### 4.5 `RulesetDef` (Domain)

Purpose: Immutable definition of sporting, technical, and session rules for a series.

| Field | Type | Unit / Convention | Constraints |
|---|---|---|---|
| `Id` | `RulesetId` | Identifier | Unique |
| `DisplayName` | `string` | Text | Non-empty |
| `PointsTable` | `IReadOnlyList<int>` | Ordered positions | Non-empty, all values `>= 0` |
| `FastestLapBonusPoints` | `int` | Points | `>= 0` |
| `AvailableCompounds` | `IReadOnlyList<TyreCompoundId>` | Ordered compounds | All compounds exist in pack |
| `MinCompoundsPerRace` | `int` | Count | `> 0` and `<= AvailableCompounds.Count` |
| `RefuellingAllowed` | `bool` | Flag | — |
| `AvailableEngineModes` | `IReadOnlyList<EngineModeId>` | Ordered modes | All modes exist in `ScalarTables.EngineModes` |
| `PartLimitPerCategory` | `int` | Count | `> 0` |
| `BudgetCapCents` | `long` | Cents | `0` means no cap |
| `PitLaneTimeLossMs` | `int` | Milliseconds | `> 0` |
| `PitStopBaseTimeMs` | `int` | Milliseconds | `> 0` |
| `QualifyingFormat` | `QualifyingFormat` | Enum | Valid enum |
| `SprintRaceEnabled` | `bool` | Flag | — |
| `SprintPointsTable` | `IReadOnlyList<int>` | Ordered positions | Required when sprint enabled, all values `>= 0` |
| `DrsDisabledLaps` | `int` | Laps | `>= 0` |
| `DrsDisabledInWet` | `bool` | Flag | — |

Illustrative JSON:

```json
{
  "id": "f1_2030_rules",
  "displayName": "2030 F1 Regulations",
  "pointsTable": [25, 18, 15, 12, 10, 8, 6, 4, 2, 1],
  "fastestLapBonusPoints": 1,
  "availableCompounds": ["soft", "medium", "hard", "inter", "wet"],
  "minCompoundsPerRace": 2,
  "refuellingAllowed": false,
  "availableEngineModes": ["push", "standard", "conserve"],
  "partLimitPerCategory": 3,
  "budgetCapCents": 14000000000,
  "pitLaneTimeLossMs": 20000,
  "pitStopBaseTimeMs": 2500,
  "qualifyingFormat": "ThreeKnockout",
  "sprintRaceEnabled": false,
  "sprintPointsTable": [],
  "drsDisabledLaps": 2,
  "drsDisabledInWet": true
}
```

### 4.6 `TrackDef` (Domain)

Purpose: Immutable definition of a track layout and its race-relevant characteristics.

| Field | Type | Unit / Convention | Constraints |
|---|---|---|---|
| `Id` | `TrackId` | Identifier | Unique |
| `DisplayName` | `string` | Text | Non-empty |
| `Country` | `string` | Text | Non-empty |
| `LengthMeters` | `int` | Meters | `> 0` |
| `BaseTimeMs` | `int` | Milliseconds | `> 0`, equals sum of sector base times |
| `TotalLaps` | `int` | Laps | `> 0` |
| `Corners` | `int` | Count | `>= 0` |
| `Sectors` | `IReadOnlyList<SectorDef>` | Ordered sectors | Non-empty |
| `OvertakingDifficultyRatingBasisPoints` | `int` | Basis points | `0..10000` |
| `TyreWearMultiplierBasisPoints` | `int` | Basis points | `10000 = 1.0x` |
| `FuelConsumptionKgPerLap` | `int` | Kilograms | `> 0` |
| `WeatherProbabilityBasisPoints` | `IReadOnlyDictionary<WeatherTypeId, int>` | Basis points by weather | Keys exist in pack, values sum to `10000` |
| `AmbientTemperatureBaseCelsius` | `int` | Celsius | Any integer |
| `TrackTemperatureBaseCelsius` | `int` | Celsius | Any integer |
| `DrsZoneSectorIndices` | `IReadOnlyList<int>` | Sector indices | Valid sector indices |

`SectorDef` (`readonly struct`):

| Field | Type | Unit / Convention | Constraints |
|---|---|---|---|
| `Index` | `int` | 0-based order | Sequential |
| `BaseTimeMs` | `int` | Milliseconds | `> 0` |
| `StraightPercentBasisPoints` | `int` | Basis points | `0..10000` |
| `CornerCount` | `int` | Count | `>= 0` |

Illustrative JSON:

```json
{
  "id": "monaco",
  "displayName": "Monaco Grand Prix",
  "country": "Monaco",
  "lengthMeters": 3337,
  "baseTimeMs": 73000,
  "totalLaps": 78,
  "corners": 19,
  "sectors": [
    { "index": 0, "baseTimeMs": 25000, "straightPercentBasisPoints": 2000, "cornerCount": 7 },
    { "index": 1, "baseTimeMs": 24000, "straightPercentBasisPoints": 3000, "cornerCount": 6 },
    { "index": 2, "baseTimeMs": 24000, "straightPercentBasisPoints": 4000, "cornerCount": 6 }
  ],
  "overtakingDifficultyRatingBasisPoints": 9200,
  "tyreWearMultiplierBasisPoints": 7500,
  "fuelConsumptionKgPerLap": 2,
  "weatherProbabilityBasisPoints": { "dry": 7000, "damp": 1500, "wet": 1000, "monsoon": 500 },
  "ambientTemperatureBaseCelsius": 22,
  "trackTemperatureBaseCelsius": 38,
  "drsZoneSectorIndices": [2]
}
```

### 4.7 `PartTypeDef` (Domain)

Purpose: Immutable definition of a part category archetype used to generate concrete runtime parts.

| Field | Type | Unit / Convention | Constraints |
|---|---|---|---|
| `Id` | `PartTypeId` | Identifier | Unique |
| `DisplayName` | `string` | Text | Non-empty |
| `Slot` | `PartSlot` | Enum | Valid enum |
| `StatMinBasisPoints` | `int` | Basis points | `0..10000`, `<= StatMaxBasisPoints` |
| `StatMaxBasisPoints` | `int` | Basis points | `0..10000`, `>= StatMinBasisPoints` |
| `BuildTimeBaseWeeks` | `int` | Weeks | `> 0` |
| `BuildCostBaseCents` | `long` | Cents | `> 0` |
| `WeightGrams` | `int` | Grams | `>= 0` |

Illustrative JSON:

```json
{
  "id": "engine_v1",
  "displayName": "Standard Engine",
  "slot": "Engine",
  "statMinBasisPoints": 3000,
  "statMaxBasisPoints": 9500,
  "buildTimeBaseWeeks": 4,
  "buildCostBaseCents": 500000000,
  "weightGrams": 0
}
```

### 4.8 `SponsorDef` (Domain)

Purpose: Immutable definition of a sponsor template and its requirement bands.

| Field | Type | Unit / Convention | Constraints |
|---|---|---|---|
| `Id` | `SponsorId` | Identifier | Unique |
| `DisplayName` | `string` | Text | Non-empty |
| `Tier` | `SponsorTier` | Enum | Valid enum |
| `PaymentPerRaceCents` | `long` | Cents | `>= 0` |
| `SigningBonusCents` | `long` | Cents | `>= 0` |
| `MinTeamReputationBasisPoints` | `int` | Basis points | `0..10000` |
| `MinChampionshipPosition` | `int` | Position | `0` means no requirement |
| `ObjectiveTemplates` | `IReadOnlyList<SponsorObjectiveTemplate>` | Ordered templates | Non-empty |

`SponsorObjectiveTemplate` (`readonly struct`):

| Field | Type | Unit / Convention | Constraints |
|---|---|---|---|
| `Description` | `string` | Text | Non-empty |
| `TargetValue` | `int` | Metric-specific integer | Any integer |
| `MetricKey` | `string` | Stable metric key | Non-empty |
| `PenaltyCents` | `long` | Cents | `>= 0` |

Illustrative JSON:

```json
{
  "id": "megacorp",
  "displayName": "MegaCorp Industries",
  "tier": "Title",
  "paymentPerRaceCents": 200000000,
  "signingBonusCents": 500000000,
  "minTeamReputationBasisPoints": 7000,
  "minChampionshipPosition": 5,
  "objectiveTemplates": [
    {
      "description": "Finish in top 5 in constructors",
      "targetValue": 5,
      "metricKey": "constructors_position",
      "penaltyCents": 100000000
    }
  ]
}
```

### 4.9 `BuildingDef` (Domain)

Purpose: Immutable definition of a facility and its ordered upgrade levels.

| Field | Type | Unit / Convention | Constraints |
|---|---|---|---|
| `Id` | `BuildingId` | Identifier | Unique |
| `DisplayName` | `string` | Text | Non-empty |
| `Levels` | `IReadOnlyList<BuildingLevelDef>` | Ordered by `Level` | Non-empty |

`BuildingLevelDef` (`sealed class`):

| Field | Type | Unit / Convention | Constraints |
|---|---|---|---|
| `Level` | `int` | 1-based level | Contiguous from 1 |
| `UpgradeCostCents` | `long` | Cents | `>= 0` |
| `UpgradeTimeWeeks` | `int` | Weeks | `> 0` |
| `Effects` | `IReadOnlyList<BuildingEffectDef>` | Ordered effects | Non-empty |

`BuildingEffectDef` (`readonly struct`):

| Field | Type | Unit / Convention | Constraints |
|---|---|---|---|
| `EffectType` | `BuildingEffectType` | Enum | Valid enum |
| `ValueBasisPoints` | `int` | Basis points | `0..10000` |

### 4.10 `ComponentDef` (Domain)

Purpose: Immutable definition of a component archetype that modifies part characteristics.

| Field | Type | Unit / Convention | Constraints |
|---|---|---|---|
| `Id` | `ComponentId` | Identifier | Unique |
| `DisplayName` | `string` | Text | Non-empty |
| `TargetSlot` | `PartSlot` | Enum | Valid enum |
| `StatContributionMinBasisPoints` | `int` | Basis points | `0..10000`, `<= Max` |
| `StatContributionMaxBasisPoints` | `int` | Basis points | `0..10000`, `>= Min` |
| `ReliabilityContributionMinBasisPoints` | `int` | Basis points | `0..10000`, `<= Max` |
| `ReliabilityContributionMaxBasisPoints` | `int` | Basis points | `0..10000`, `>= Min` |
| `BuildTimeModifierWeeks` | `int` | Weeks | Added to part base |
| `CostModifierCents` | `long` | Cents | Added to part base |

### 4.11 `DriverArchetypeDef` (Domain)

Purpose: Immutable definition of driver generation ranges for world and career creation.

| Field | Type | Unit / Convention | Constraints |
|---|---|---|---|
| `Id` | `DriverArchetypeId` | Identifier | Unique |
| `DisplayName` | `string` | Text | Non-empty |
| `SpeedMinBasisPoints` / `SpeedMaxBasisPoints` | `int` | Basis points | `0..10000`, min `<=` max |
| `ConsistencyMinBasisPoints` / `ConsistencyMaxBasisPoints` | `int` | Basis points | `0..10000`, min `<=` max |
| `WetSkillMinBasisPoints` / `WetSkillMaxBasisPoints` | `int` | Basis points | `0..10000`, min `<=` max |
| `OvertakingMinBasisPoints` / `OvertakingMaxBasisPoints` | `int` | Basis points | `0..10000`, min `<=` max |
| `DefenceMinBasisPoints` / `DefenceMaxBasisPoints` | `int` | Basis points | `0..10000`, min `<=` max |
| `AgeMin` / `AgeMax` | `int` | Years | `> 0`, min `<=` max |
| `StartingReputationMinBasisPoints` / `StartingReputationMaxBasisPoints` | `int` | Basis points | `0..10000`, min `<=` max |

## 5. Definition Pack & Registry

`DefinitionPack` is the complete set of immutable definitions for one game configuration:

```text
DefinitionPack
    PackId: string
    PackVersion: string (semver)
    Series: IReadOnlyList<SeriesDef>
    Rulesets: IReadOnlyList<RulesetDef>
    Tracks: IReadOnlyList<TrackDef>
    PartTypes: IReadOnlyList<PartTypeDef>
    TyreCompounds: IReadOnlyList<TyreCompoundDef>
    Sponsors: IReadOnlyList<SponsorDef>
    Buildings: IReadOnlyList<BuildingDef>
    Components: IReadOnlyList<ComponentDef>
    DriverArchetypes: IReadOnlyList<DriverArchetypeDef>
    WeatherTypes: IReadOnlyList<WeatherDef>
    Scalars: ScalarTables
```

`IDefinitionRegistry` is the type-safe lookup facade:

- `Get{Type}({TypeId} id)` returns the definition or throws `KeyNotFoundException` if missing.
- `All{Type}s` returns the full list in pack order.
- `Scalars` returns `ScalarTables`.

`IDefinitionPackLoader` lives in the Domain layer and loads a pack from a directory path. The concrete loader belongs to the Persistence layer in a separate task.

`IDefinitionValidator` validates a pack and returns `ValidationResult` with `IsValid` and `Errors`.

## 6. Cross-Reference Integrity Rules

| Source | Referenced Type | Must Exist In |
|---|---|---|
| `SeriesDef.RulesetId` | `RulesetId` | `DefinitionPack.Rulesets` |
| `SeriesDef.CalendarTemplate[*]` | `TrackId` | `DefinitionPack.Tracks` |
| `RulesetDef.AvailableCompounds[*]` | `TyreCompoundId` | `DefinitionPack.TyreCompounds` |
| `RulesetDef.AvailableEngineModes[*]` | `EngineModeId` | `ScalarTables.EngineModes` |
| `TrackDef.WeatherProbabilityBasisPoints` keys | `WeatherTypeId` | `DefinitionPack.WeatherTypes` |
| `WeatherDef.CompoundMatchPenaltyMs` keys | `TyreCompoundId` | `DefinitionPack.TyreCompounds` |
| `WeatherDef.TransitionProbabilityBasisPoints` keys | `WeatherTypeId` | `DefinitionPack.WeatherTypes` |

## 7. Definition Pack File Layout

```text
packs/
  f1_2030/
    pack.json                     -- { "packId": "f1_2030", "packVersion": "1.0.0" }
    series.json                   -- SeriesDef[]
    rulesets.json                 -- RulesetDef[]
    tracks/
      monaco.json                 -- TrackDef (one per file)
      silverstone.json
    part_types.json               -- PartTypeDef[]
    tyre_compounds.json           -- TyreCompoundDef[]
    sponsors.json                 -- SponsorDef[]
    buildings.json                -- BuildingDef[]
    components.json               -- ComponentDef[]
    driver_archetypes.json        -- DriverArchetypeDef[]
    weather.json                  -- WeatherDef[]
    scalars/
      part_rank_time_cost.json
      driver_pace_scalars.json
      setup_scalars.json
      knowledge_scalars.json
      tyre_temperature_scalars.json
      fuel_scalars.json
      engine_mode_defs.json
      drafting_scalars.json
      variance_scalars.json
```

## 8. Invariants & Bounds

| Invariant | Rule |
|---|---|
| All ids unique within type | No two definitions of the same type share an id |
| Cross-references valid | Every referenced id must exist in the pack |
| Basis point fields bounded | `0..10000` unless explicitly documented otherwise |
| Min `<=` max for all range pairs | Applies to stats, age ranges, and all definition range pairs |
| Probability sums | Weather transition and track weather probabilities sum to exactly `10000` |
| Sector time sum | Sum of sector `BaseTimeMs` equals track `BaseTimeMs` |
| Rank tables monotonic | Part rank time cost arrays are monotonically non-decreasing |
| No floats | All numeric fields use integer types |
| At least one series | Pack contains one or more `SeriesDef` entries |
| Non-empty required collections | `PointsTable`, `CalendarTemplate`, `Sectors`, `Levels`, and other required collections are non-empty |

## 9. Verification Strategy

| Test | Input | Expected |
|---|---|---|
| Valid pack loads | Complete consistent test pack | Validation passes, all lookups work |
| Duplicate id rejected | Two tracks with same `TrackId` | Validation error |
| Broken cross-reference rejected | Series references missing track | Validation error |
| Sector sum mismatch rejected | Sector times do not sum to track base time | Validation error |
| Pack swap works | Load `test_f1`, then `test_indycar` | Both validate independently, no shared state |
| Rank lookup clamps | Rank beyond table length | Returns last entry |
| Bounds violation rejected | `GripRatingBasisPoints = 15000` | Validation error |
| Float-free guarantee | Static scan of all definition type fields | No `float` or `double` fields found |
