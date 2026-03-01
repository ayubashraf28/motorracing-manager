using System.Collections.Generic;
using System.Linq;
using MotorracingManager.Core.Definitions;
using MotorracingManager.Core.Definitions.Scalars;
using MotorracingManager.Core.Enums;
using MotorracingManager.Core.Identifiers;
using MotorracingManager.Domain.Definitions;
using MotorracingManager.Domain.Tests.TestData;
using NUnit.Framework;

namespace MotorracingManager.Domain.Tests.Definitions
{
    public sealed class DefinitionPackValidatorTests
    {
        private readonly DefinitionPackValidator _validator = new DefinitionPackValidator();

        [Test]
        public void Validate_AllowsValidF1Pack()
        {
            var result = _validator.Validate(TestDefinitionPackBuilder.CreateF1Pack());

            Assert.That(result.IsValid, Is.True, string.Join(", ", result.Errors));
        }

        [Test]
        public void Validate_AllowsValidIndyCarPack()
        {
            var result = _validator.Validate(TestDefinitionPackBuilder.CreateIndyCarPack());

            Assert.That(result.IsValid, Is.True, string.Join(", ", result.Errors));
        }

        [Test]
        public void Validate_RejectsDuplicateTrackId()
        {
            var pack = TestDefinitionPackBuilder.CreateF1Pack();
            var duplicateTrack = new TrackDef(new TrackId("monaco"), "Clone Monaco", "Monaco", 3337, 73000, 78, 19,
                new[] { new SectorDef(0, 25000, 2000, 7), new SectorDef(1, 24000, 3000, 6), new SectorDef(2, 24000, 4000, 6) },
                9200, 7500, 2, new Dictionary<WeatherTypeId, int> { { new WeatherTypeId("dry"), 7000 }, { new WeatherTypeId("damp"), 1500 }, { new WeatherTypeId("wet"), 1000 }, { new WeatherTypeId("monsoon"), 500 } }, 22, 38, new[] { 2 });
            var result = _validator.Validate(TestDefinitionPackBuilder.Clone(pack, tracks: new[] { pack.Tracks[0], duplicateTrack }));

            AssertInvalid(result, "Duplicate TrackDef id");
        }

        [Test]
        public void Validate_RejectsBrokenRulesetReference()
        {
            var pack = TestDefinitionPackBuilder.CreateF1Pack();
            var series = new SeriesDef(new SeriesId("f1"), "Formula 1 Championship", 1, new RulesetId("missing_rules"), pack.Series[0].CalendarTemplate, 10, 2);
            var result = _validator.Validate(TestDefinitionPackBuilder.Clone(pack, series: new[] { series }));

            AssertInvalid(result, ".RulesetId references missing RulesetId");
        }

        [Test]
        public void Validate_RejectsBrokenTrackReference()
        {
            var pack = TestDefinitionPackBuilder.CreateF1Pack();
            var series = new SeriesDef(new SeriesId("f1"), "Formula 1 Championship", 1, pack.Series[0].RulesetId, new[] { new TrackId("missing_track") }, 10, 2);
            var result = _validator.Validate(TestDefinitionPackBuilder.Clone(pack, series: new[] { series }));

            AssertInvalid(result, ".CalendarTemplate references missing TrackId");
        }

        [Test]
        public void Validate_RejectsBrokenTyreCompoundReference()
        {
            var pack = TestDefinitionPackBuilder.CreateF1Pack();
            var ruleset = new RulesetDef(pack.Rulesets[0].Id, pack.Rulesets[0].DisplayName, pack.Rulesets[0].PointsTable, pack.Rulesets[0].FastestLapBonusPoints,
                new[] { new TyreCompoundId("missing_compound") }, 1, pack.Rulesets[0].RefuellingAllowed, pack.Rulesets[0].AvailableEngineModes, pack.Rulesets[0].PartLimitPerCategory,
                pack.Rulesets[0].BudgetCapCents, pack.Rulesets[0].PitLaneTimeLossMs, pack.Rulesets[0].PitStopBaseTimeMs, pack.Rulesets[0].QualifyingFormat,
                pack.Rulesets[0].SprintRaceEnabled, pack.Rulesets[0].SprintPointsTable, pack.Rulesets[0].DrsDisabledLaps, pack.Rulesets[0].DrsDisabledInWet);
            var result = _validator.Validate(TestDefinitionPackBuilder.Clone(pack, rulesets: new[] { ruleset }));

            AssertInvalid(result, ".AvailableCompounds references missing TyreCompoundId");
        }

        [Test]
        public void Validate_RejectsBrokenWeatherReference()
        {
            var pack = TestDefinitionPackBuilder.CreateF1Pack();
            var track = new TrackDef(pack.Tracks[0].Id, pack.Tracks[0].DisplayName, pack.Tracks[0].Country, pack.Tracks[0].LengthMeters, pack.Tracks[0].BaseTimeMs, pack.Tracks[0].TotalLaps,
                pack.Tracks[0].Corners, pack.Tracks[0].Sectors, pack.Tracks[0].OvertakingDifficultyRatingBasisPoints, pack.Tracks[0].TyreWearMultiplierBasisPoints, pack.Tracks[0].FuelConsumptionKgPerLap,
                new Dictionary<WeatherTypeId, int> { { new WeatherTypeId("hail"), 10000 } }, pack.Tracks[0].AmbientTemperatureBaseCelsius, pack.Tracks[0].TrackTemperatureBaseCelsius, pack.Tracks[0].DrsZoneSectorIndices);
            var result = _validator.Validate(TestDefinitionPackBuilder.Clone(pack, tracks: new[] { track, pack.Tracks[1] }));

            AssertInvalid(result, ".WeatherProbabilityBasisPoints references missing WeatherTypeId");
        }

        [Test]
        public void Validate_RejectsSectorTimeMismatch()
        {
            var pack = TestDefinitionPackBuilder.CreateF1Pack();
            var track = new TrackDef(pack.Tracks[0].Id, pack.Tracks[0].DisplayName, pack.Tracks[0].Country, pack.Tracks[0].LengthMeters, 72000, pack.Tracks[0].TotalLaps,
                pack.Tracks[0].Corners, pack.Tracks[0].Sectors, pack.Tracks[0].OvertakingDifficultyRatingBasisPoints, pack.Tracks[0].TyreWearMultiplierBasisPoints, pack.Tracks[0].FuelConsumptionKgPerLap,
                pack.Tracks[0].WeatherProbabilityBasisPoints, pack.Tracks[0].AmbientTemperatureBaseCelsius, pack.Tracks[0].TrackTemperatureBaseCelsius, pack.Tracks[0].DrsZoneSectorIndices);
            var result = _validator.Validate(TestDefinitionPackBuilder.Clone(pack, tracks: new[] { track, pack.Tracks[1] }));

            AssertInvalid(result, ".BaseTimeMs must equal the sum of sector BaseTimeMs.");
        }

        [Test]
        public void Validate_RejectsTrackWeatherProbabilitySumMismatch()
        {
            var pack = TestDefinitionPackBuilder.CreateF1Pack();
            var track = new TrackDef(pack.Tracks[0].Id, pack.Tracks[0].DisplayName, pack.Tracks[0].Country, pack.Tracks[0].LengthMeters, pack.Tracks[0].BaseTimeMs, pack.Tracks[0].TotalLaps,
                pack.Tracks[0].Corners, pack.Tracks[0].Sectors, pack.Tracks[0].OvertakingDifficultyRatingBasisPoints, pack.Tracks[0].TyreWearMultiplierBasisPoints, pack.Tracks[0].FuelConsumptionKgPerLap,
                new Dictionary<WeatherTypeId, int> { { new WeatherTypeId("dry"), 5000 }, { new WeatherTypeId("damp"), 2000 }, { new WeatherTypeId("wet"), 1000 } },
                pack.Tracks[0].AmbientTemperatureBaseCelsius, pack.Tracks[0].TrackTemperatureBaseCelsius, pack.Tracks[0].DrsZoneSectorIndices);
            var result = _validator.Validate(TestDefinitionPackBuilder.Clone(pack, tracks: new[] { track, pack.Tracks[1] }));

            AssertInvalid(result, ".WeatherProbabilityBasisPoints must sum to exactly 10000.");
        }

        [Test]
        public void Validate_RejectsWeatherTransitionProbabilitySumMismatch()
        {
            var pack = TestDefinitionPackBuilder.CreateF1Pack();
            var weather = new WeatherDef(pack.WeatherTypes[0].Id, pack.WeatherTypes[0].DisplayName, pack.WeatherTypes[0].State, pack.WeatherTypes[0].BasePenaltyMs, pack.WeatherTypes[0].GripModifierBasisPoints,
                pack.WeatherTypes[0].CompoundMatchPenaltyMs,
                new Dictionary<WeatherTypeId, int> { { new WeatherTypeId("dry"), 6000 }, { new WeatherTypeId("damp"), 1000 }, { new WeatherTypeId("wet"), 1000 }, { new WeatherTypeId("monsoon"), 500 } },
                pack.WeatherTypes[0].TrackTemperatureDeltaCelsius);
            var result = _validator.Validate(TestDefinitionPackBuilder.Clone(pack, weatherTypes: new[] { weather, pack.WeatherTypes[1], pack.WeatherTypes[2], pack.WeatherTypes[3] }));

            AssertInvalid(result, ".TransitionProbabilityBasisPoints must sum to exactly 10000.");
        }

        [Test]
        public void Validate_RejectsBasisPointOverflow()
        {
            var pack = TestDefinitionPackBuilder.CreateF1Pack();
            var tyre = new TyreCompoundDef(pack.TyreCompounds[0].Id, pack.TyreCompounds[0].DisplayName, 15000, pack.TyreCompounds[0].WearRatePerLapBasisPoints, pack.TyreCompounds[0].FreshLaps,
                pack.TyreCompounds[0].OptimalLaps, pack.TyreCompounds[0].WornLaps, pack.TyreCompounds[0].CliffMsPerLap, pack.TyreCompounds[0].OptimalBonusMs, pack.TyreCompounds[0].FreshPenaltyMs,
                pack.TyreCompounds[0].TemperatureWindowLowCelsius, pack.TyreCompounds[0].TemperatureWindowHighCelsius, pack.TyreCompounds[0].WarmupRateCelsiusPerLap);
            var result = _validator.Validate(TestDefinitionPackBuilder.Clone(pack, tyreCompounds: new[] { tyre, pack.TyreCompounds[1], pack.TyreCompounds[2], pack.TyreCompounds[3], pack.TyreCompounds[4] }));

            AssertInvalid(result, ".GripRatingBasisPoints must be between 0 and 10000.");
        }

        [Test]
        public void Validate_RejectsMinMaxInversion()
        {
            var pack = TestDefinitionPackBuilder.CreateF1Pack();
            var component = new ComponentDef(pack.Components[0].Id, pack.Components[0].DisplayName, pack.Components[0].TargetSlot, 1800, 1200, pack.Components[0].ReliabilityContributionMinBasisPoints,
                pack.Components[0].ReliabilityContributionMaxBasisPoints, pack.Components[0].BuildTimeModifierWeeks, pack.Components[0].CostModifierCents);
            var result = _validator.Validate(TestDefinitionPackBuilder.Clone(pack, components: new[] { component, pack.Components[1] }));

            AssertInvalid(result, "stat contribution range must satisfy min <= max");
        }

        [Test]
        public void Validate_RejectsSprintEnabledWithoutSprintPoints()
        {
            var pack = TestDefinitionPackBuilder.CreateF1Pack();
            var ruleset = new RulesetDef(pack.Rulesets[0].Id, pack.Rulesets[0].DisplayName, pack.Rulesets[0].PointsTable, pack.Rulesets[0].FastestLapBonusPoints, pack.Rulesets[0].AvailableCompounds,
                pack.Rulesets[0].MinCompoundsPerRace, pack.Rulesets[0].RefuellingAllowed, pack.Rulesets[0].AvailableEngineModes, pack.Rulesets[0].PartLimitPerCategory, pack.Rulesets[0].BudgetCapCents,
                pack.Rulesets[0].PitLaneTimeLossMs, pack.Rulesets[0].PitStopBaseTimeMs, pack.Rulesets[0].QualifyingFormat, true, new int[0], pack.Rulesets[0].DrsDisabledLaps, pack.Rulesets[0].DrsDisabledInWet);
            var result = _validator.Validate(TestDefinitionPackBuilder.Clone(pack, rulesets: new[] { ruleset }));

            AssertInvalid(result, ".SprintPointsTable must be non-empty when SprintRaceEnabled is true.");
        }

        [Test]
        public void Validate_RejectsSprintDisabledWithSprintPoints()
        {
            var pack = TestDefinitionPackBuilder.CreateF1Pack();
            var ruleset = new RulesetDef(pack.Rulesets[0].Id, pack.Rulesets[0].DisplayName, pack.Rulesets[0].PointsTable, pack.Rulesets[0].FastestLapBonusPoints, pack.Rulesets[0].AvailableCompounds,
                pack.Rulesets[0].MinCompoundsPerRace, pack.Rulesets[0].RefuellingAllowed, pack.Rulesets[0].AvailableEngineModes, pack.Rulesets[0].PartLimitPerCategory, pack.Rulesets[0].BudgetCapCents,
                pack.Rulesets[0].PitLaneTimeLossMs, pack.Rulesets[0].PitStopBaseTimeMs, pack.Rulesets[0].QualifyingFormat, false, new[] { 3, 2, 1 }, pack.Rulesets[0].DrsDisabledLaps, pack.Rulesets[0].DrsDisabledInWet);
            var result = _validator.Validate(TestDefinitionPackBuilder.Clone(pack, rulesets: new[] { ruleset }));

            AssertInvalid(result, ".SprintPointsTable must be empty when SprintRaceEnabled is false.");
        }

        [Test]
        public void Validate_RejectsNonMonotonicPartRankTable()
        {
            var pack = TestDefinitionPackBuilder.CreateF1Pack();
            var scalars = new ScalarTables(
                new PartRankTimeCostTable(new Dictionary<PartSlot, IReadOnlyList<int>>
                {
                    { PartSlot.Engine, new[] { 0, 80, 60 } },
                    { PartSlot.Aero, new[] { 0, 70, 140 } },
                    { PartSlot.Chassis, new[] { 0, 60, 120 } },
                }),
                pack.Scalars.DriverPace,
                pack.Scalars.Setup,
                pack.Scalars.Knowledge,
                pack.Scalars.TyreTemperature,
                pack.Scalars.Fuel,
                pack.Scalars.EngineModes,
                pack.Scalars.Drafting,
                pack.Scalars.Variance);
            var result = _validator.Validate(TestDefinitionPackBuilder.Clone(pack, scalars: scalars));

            AssertInvalid(result, "monotonically non-decreasing");
        }

        [Test]
        public void Validate_RejectsMissingPartSlotRankTable()
        {
            var pack = TestDefinitionPackBuilder.CreateF1Pack();
            var scalars = new ScalarTables(
                new PartRankTimeCostTable(new Dictionary<PartSlot, IReadOnlyList<int>>
                {
                    { PartSlot.Engine, new[] { 0, 80, 160 } },
                    { PartSlot.Aero, new[] { 0, 70, 140 } },
                }),
                pack.Scalars.DriverPace,
                pack.Scalars.Setup,
                pack.Scalars.Knowledge,
                pack.Scalars.TyreTemperature,
                pack.Scalars.Fuel,
                pack.Scalars.EngineModes,
                pack.Scalars.Drafting,
                pack.Scalars.Variance);
            var result = _validator.Validate(TestDefinitionPackBuilder.Clone(pack, scalars: scalars));

            AssertInvalid(result, "missing slot 'Chassis'");
        }

        [Test]
        public void Validate_RejectsInvalidDrsSectorIndex()
        {
            var pack = TestDefinitionPackBuilder.CreateF1Pack();
            var track = new TrackDef(pack.Tracks[0].Id, pack.Tracks[0].DisplayName, pack.Tracks[0].Country, pack.Tracks[0].LengthMeters, pack.Tracks[0].BaseTimeMs, pack.Tracks[0].TotalLaps,
                pack.Tracks[0].Corners, pack.Tracks[0].Sectors, pack.Tracks[0].OvertakingDifficultyRatingBasisPoints, pack.Tracks[0].TyreWearMultiplierBasisPoints, pack.Tracks[0].FuelConsumptionKgPerLap,
                pack.Tracks[0].WeatherProbabilityBasisPoints, pack.Tracks[0].AmbientTemperatureBaseCelsius, pack.Tracks[0].TrackTemperatureBaseCelsius, new[] { 6 });
            var result = _validator.Validate(TestDefinitionPackBuilder.Clone(pack, tracks: new[] { track, pack.Tracks[1] }));

            AssertInvalid(result, ".DrsZoneSectorIndices must reference valid sector indices.");
        }

        [Test]
        public void Validate_RejectsInvalidSemVer()
        {
            var pack = TestDefinitionPackBuilder.CreateF1Pack();
            var result = _validator.Validate(TestDefinitionPackBuilder.Clone(pack, packVersion: "version-one"));

            AssertInvalid(result, "not valid semver");
        }

        private static void AssertInvalid(ValidationResult result, string expectedFragment)
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(message => message.Contains(expectedFragment)), Is.True, string.Join(", ", result.Errors));
        }
    }
}
