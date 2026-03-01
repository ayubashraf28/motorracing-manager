using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using MotorracingManager.Core.Definitions;
using MotorracingManager.Core.Definitions.Scalars;
using MotorracingManager.Core.Enums;
using MotorracingManager.Core.Identifiers;

namespace MotorracingManager.Domain.Definitions
{
    public sealed class DefinitionPackValidator : IDefinitionValidator
    {
        private static readonly Regex SemVerRegex = new Regex(
            "^(0|[1-9]\\d*)\\.(0|[1-9]\\d*)\\.(0|[1-9]\\d*)(?:-[0-9A-Za-z-]+(?:\\.[0-9A-Za-z-]+)*)?(?:\\+[0-9A-Za-z-]+(?:\\.[0-9A-Za-z-]+)*)?$",
            RegexOptions.Compiled);

        public ValidationResult Validate(DefinitionPack pack)
        {
            var errors = new List<string>();

            if (pack == null)
            {
                errors.Add("DefinitionPack cannot be null.");
                return new ValidationResult(errors);
            }

            ValidateMetadata(pack, errors);
            ValidateRequiredCollections(pack, errors);
            ValidateTypeShape(errors);
            ValidateUniqueness(pack, errors);
            ValidateScalarTables(pack.Scalars, errors);
            ValidateTyreCompounds(pack.TyreCompounds, errors);
            ValidateWeather(pack, errors);
            ValidateRulesets(pack, errors);
            ValidateTracks(pack, errors);
            ValidateSeries(pack, errors);
            ValidatePartTypes(pack.PartTypes, errors);
            ValidateSponsors(pack.Sponsors, errors);
            ValidateBuildings(pack.Buildings, errors);
            ValidateComponents(pack.Components, errors);
            ValidateDriverArchetypes(pack.DriverArchetypes, errors);

            return new ValidationResult(errors);
        }

        private static void ValidateMetadata(DefinitionPack pack, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(pack.PackId))
            {
                errors.Add("PackId must be non-empty.");
            }

            if (string.IsNullOrWhiteSpace(pack.PackVersion))
            {
                errors.Add("PackVersion must be non-empty.");
            }
            else if (!SemVerRegex.IsMatch(pack.PackVersion))
            {
                errors.Add(string.Format("PackVersion '{0}' is not valid semver.", pack.PackVersion));
            }
        }

        private static void ValidateRequiredCollections(DefinitionPack pack, List<string> errors)
        {
            RequireNonEmpty(pack.Series, "Series", errors);
            RequireNonEmpty(pack.Rulesets, "Rulesets", errors);
            RequireNonEmpty(pack.Tracks, "Tracks", errors);
            RequireNonEmpty(pack.PartTypes, "PartTypes", errors);
            RequireNonEmpty(pack.TyreCompounds, "TyreCompounds", errors);
            RequireNonEmpty(pack.Sponsors, "Sponsors", errors);
            RequireNonEmpty(pack.Buildings, "Buildings", errors);
            RequireNonEmpty(pack.Components, "Components", errors);
            RequireNonEmpty(pack.DriverArchetypes, "DriverArchetypes", errors);
            RequireNonEmpty(pack.WeatherTypes, "WeatherTypes", errors);
            if (pack.Scalars == null)
            {
                errors.Add("Scalars must be provided.");
            }
        }

        private static void ValidateUniqueness(DefinitionPack pack, List<string> errors)
        {
            ValidateUniqueIds(pack.Series, value => value.Id, "SeriesDef", errors);
            ValidateUniqueIds(pack.Rulesets, value => value.Id, "RulesetDef", errors);
            ValidateUniqueIds(pack.Tracks, value => value.Id, "TrackDef", errors);
            ValidateUniqueIds(pack.PartTypes, value => value.Id, "PartTypeDef", errors);
            ValidateUniqueIds(pack.TyreCompounds, value => value.Id, "TyreCompoundDef", errors);
            ValidateUniqueIds(pack.Sponsors, value => value.Id, "SponsorDef", errors);
            ValidateUniqueIds(pack.Buildings, value => value.Id, "BuildingDef", errors);
            ValidateUniqueIds(pack.Components, value => value.Id, "ComponentDef", errors);
            ValidateUniqueIds(pack.DriverArchetypes, value => value.Id, "DriverArchetypeDef", errors);
            ValidateUniqueIds(pack.WeatherTypes, value => value.Id, "WeatherDef", errors);
        }

        private static void ValidateScalarTables(ScalarTables scalars, List<string> errors)
        {
            if (scalars == null)
            {
                return;
            }

            if (scalars.PartRankTimeCostTable == null)
            {
                errors.Add("PartRankTimeCostTable is required.");
            }
            else
            {
                foreach (PartSlot slot in Enum.GetValues(typeof(PartSlot)))
                {
                    if (!scalars.PartRankTimeCostTable.RankToMsByCategory.TryGetValue(slot, out var values))
                    {
                        errors.Add(string.Format("PartRankTimeCostTable is missing slot '{0}'.", slot));
                        continue;
                    }

                    if (values == null || values.Count == 0)
                    {
                        errors.Add(string.Format("PartRankTimeCostTable slot '{0}' must be non-empty.", slot));
                        continue;
                    }

                    for (var index = 1; index < values.Count; index++)
                    {
                        if (values[index] < values[index - 1])
                        {
                            errors.Add(string.Format("PartRankTimeCostTable slot '{0}' must be monotonically non-decreasing.", slot));
                            break;
                        }
                    }
                }
            }

            if (scalars.DriverPace == null)
            {
                errors.Add("DriverPaceScalars are required.");
            }
            else if (scalars.DriverPace.MsPerRatingPoint <= 0)
            {
                errors.Add("DriverPaceScalars.MsPerRatingPoint must be > 0.");
            }

            if (scalars.Setup == null)
            {
                errors.Add("SetupScalars are required.");
            }
            else
            {
                RequireNonEmpty(scalars.Setup.Penalties, "SetupScalars.Penalties", errors);
                foreach (var penalty in scalars.Setup.Penalties)
                {
                    ValidateRangeInclusive(penalty.MatchPercentage, 0, 100, "SetupPenaltyEntry.MatchPercentage", errors);
                    if (penalty.PenaltyMs < 0)
                    {
                        errors.Add("SetupPenaltyEntry.PenaltyMs must be >= 0.");
                    }
                }
            }

            if (scalars.Knowledge == null)
            {
                errors.Add("KnowledgeScalars are required.");
            }
            else
            {
                if (scalars.Knowledge.MaxPenaltyMs < 0)
                {
                    errors.Add("KnowledgeScalars.MaxPenaltyMs must be >= 0.");
                }

                ValidateRangeInclusive(scalars.Knowledge.RetentionPercentPerSeason, 0, 100, "KnowledgeScalars.RetentionPercentPerSeason", errors);
            }

            if (scalars.TyreTemperature == null)
            {
                errors.Add("TyreTemperatureScalars are required.");
            }
            else if (scalars.TyreTemperature.MsPerDegreeOutsideWindow < 0)
            {
                errors.Add("TyreTemperatureScalars.MsPerDegreeOutsideWindow must be >= 0.");
            }

            if (scalars.Fuel == null)
            {
                errors.Add("FuelScalars are required.");
            }
            else if (scalars.Fuel.MsPerKg <= 0)
            {
                errors.Add("FuelScalars.MsPerKg must be > 0.");
            }

            if (scalars.Drafting == null)
            {
                errors.Add("DraftingScalars are required.");
            }
            else
            {
                if (scalars.Drafting.MaxBenefitMs < 0)
                {
                    errors.Add("DraftingScalars.MaxBenefitMs must be >= 0.");
                }

                if (scalars.Drafting.ThresholdGapMs < 0)
                {
                    errors.Add("DraftingScalars.ThresholdGapMs must be >= 0.");
                }

                if (scalars.Drafting.DrsBonusMs < 0)
                {
                    errors.Add("DraftingScalars.DrsBonusMs must be >= 0.");
                }
            }

            if (scalars.Variance == null)
            {
                errors.Add("VarianceScalars are required.");
            }
            else
            {
                if (scalars.Variance.BaseRangeMs < 0)
                {
                    errors.Add("VarianceScalars.BaseRangeMs must be >= 0.");
                }

                ValidateBasisPoints(scalars.Variance.ConsistencyAttenuationBasisPoints, "VarianceScalars.ConsistencyAttenuationBasisPoints", errors);
            }

            RequireNonEmpty(scalars.EngineModes, "ScalarTables.EngineModes", errors);
            ValidateUniqueIds(scalars.EngineModes, value => value.Id, "EngineModeScalar", errors);
            foreach (var engineMode in scalars.EngineModes)
            {
                if (engineMode.ReliabilityMultiplierBasisPoints <= 0)
                {
                    errors.Add(engineMode.Id + ".ReliabilityMultiplierBasisPoints must be > 0.");
                }

                if (engineMode.FuelEfficiencyMultiplierBasisPoints <= 0)
                {
                    errors.Add(engineMode.Id + ".FuelEfficiencyMultiplierBasisPoints must be > 0.");
                }
            }
        }

        private static void ValidateTyreCompounds(IReadOnlyList<TyreCompoundDef> compounds, List<string> errors)
        {
            foreach (var compound in compounds)
            {
                ValidateBasisPoints(compound.GripRatingBasisPoints, compound.Id + ".GripRatingBasisPoints", errors);
                if (compound.WearRatePerLapBasisPoints <= 0) errors.Add(compound.Id + ".WearRatePerLapBasisPoints must be > 0.");
                if (compound.FreshLaps < 0) errors.Add(compound.Id + ".FreshLaps must be >= 0.");
                if (compound.OptimalLaps <= 0) errors.Add(compound.Id + ".OptimalLaps must be > 0.");
                if (compound.WornLaps < 0) errors.Add(compound.Id + ".WornLaps must be >= 0.");
                if (compound.CliffMsPerLap <= 0) errors.Add(compound.Id + ".CliffMsPerLap must be > 0.");
                if (compound.OptimalBonusMs >= 0) errors.Add(compound.Id + ".OptimalBonusMs must be negative.");
                if (compound.FreshPenaltyMs <= 0) errors.Add(compound.Id + ".FreshPenaltyMs must be positive.");
                if (compound.TemperatureWindowLowCelsius >= compound.TemperatureWindowHighCelsius) errors.Add(compound.Id + " temperature window low must be less than high.");
                if (compound.WarmupRateCelsiusPerLap <= 0) errors.Add(compound.Id + ".WarmupRateCelsiusPerLap must be > 0.");
            }
        }

        private static void ValidateWeather(DefinitionPack pack, List<string> errors)
        {
            var compoundIds = new HashSet<TyreCompoundId>(pack.TyreCompounds.Select(value => value.Id));
            var weatherIds = new HashSet<WeatherTypeId>(pack.WeatherTypes.Select(value => value.Id));

            foreach (var weather in pack.WeatherTypes)
            {
                ValidateBasisPoints(weather.GripModifierBasisPoints, weather.Id + ".GripModifierBasisPoints", errors);
                if (weather.BasePenaltyMs < 0) errors.Add(weather.Id + ".BasePenaltyMs must be >= 0.");
                if (!Enum.IsDefined(typeof(WeatherState), weather.State)) errors.Add(weather.Id + ".State must be a valid WeatherState value.");
                ValidateProbabilityMap(weather.TransitionProbabilityBasisPoints, weather.Id + ".TransitionProbabilityBasisPoints", errors);

                foreach (var compound in weather.CompoundMatchPenaltyMs.Keys)
                {
                    if (!compoundIds.Contains(compound))
                    {
                        errors.Add(string.Format("{0}.CompoundMatchPenaltyMs references missing TyreCompoundId '{1}'.", weather.Id, compound));
                    }
                }

                foreach (var transition in weather.TransitionProbabilityBasisPoints.Keys)
                {
                    if (!weatherIds.Contains(transition))
                    {
                        errors.Add(string.Format("{0}.TransitionProbabilityBasisPoints references missing WeatherTypeId '{1}'.", weather.Id, transition));
                    }
                }
            }
        }

        private static void ValidateRulesets(DefinitionPack pack, List<string> errors)
        {
            var compoundIds = new HashSet<TyreCompoundId>(pack.TyreCompounds.Select(value => value.Id));
            var engineModeIds = new HashSet<EngineModeId>(pack.Scalars.EngineModes.Select(value => value.Id));

            foreach (var ruleset in pack.Rulesets)
            {
                RequireNonEmpty(ruleset.PointsTable, ruleset.Id + ".PointsTable", errors);
                RequireNonEmpty(ruleset.AvailableCompounds, ruleset.Id + ".AvailableCompounds", errors);
                RequireNonEmpty(ruleset.AvailableEngineModes, ruleset.Id + ".AvailableEngineModes", errors);
                if (ruleset.PointsTable.Any(points => points < 0)) errors.Add(ruleset.Id + ".PointsTable values must be >= 0.");
                if (ruleset.SprintPointsTable.Any(points => points < 0)) errors.Add(ruleset.Id + ".SprintPointsTable values must be >= 0.");
                if (ruleset.FastestLapBonusPoints < 0) errors.Add(ruleset.Id + ".FastestLapBonusPoints must be >= 0.");
                if (ruleset.MinCompoundsPerRace <= 0 || ruleset.MinCompoundsPerRace > ruleset.AvailableCompounds.Count) errors.Add(ruleset.Id + ".MinCompoundsPerRace must be > 0 and <= AvailableCompounds.Count.");
                if (ruleset.PartLimitPerCategory <= 0) errors.Add(ruleset.Id + ".PartLimitPerCategory must be > 0.");
                if (ruleset.BudgetCapCents < 0) errors.Add(ruleset.Id + ".BudgetCapCents must be >= 0.");
                if (ruleset.PitLaneTimeLossMs <= 0) errors.Add(ruleset.Id + ".PitLaneTimeLossMs must be > 0.");
                if (ruleset.PitStopBaseTimeMs <= 0) errors.Add(ruleset.Id + ".PitStopBaseTimeMs must be > 0.");
                if (ruleset.DrsDisabledLaps < 0) errors.Add(ruleset.Id + ".DrsDisabledLaps must be >= 0.");
                if (!Enum.IsDefined(typeof(QualifyingFormat), ruleset.QualifyingFormat)) errors.Add(ruleset.Id + ".QualifyingFormat must be valid.");

                if (ruleset.SprintRaceEnabled && ruleset.SprintPointsTable.Count == 0)
                {
                    errors.Add(ruleset.Id + ".SprintPointsTable must be non-empty when SprintRaceEnabled is true.");
                }
                else if (!ruleset.SprintRaceEnabled && ruleset.SprintPointsTable.Count > 0)
                {
                    errors.Add(ruleset.Id + ".SprintPointsTable must be empty when SprintRaceEnabled is false.");
                }

                foreach (var compoundId in ruleset.AvailableCompounds)
                {
                    if (!compoundIds.Contains(compoundId))
                    {
                        errors.Add(string.Format("{0}.AvailableCompounds references missing TyreCompoundId '{1}'.", ruleset.Id, compoundId));
                    }
                }

                foreach (var engineModeId in ruleset.AvailableEngineModes)
                {
                    if (!engineModeIds.Contains(engineModeId))
                    {
                        errors.Add(string.Format("{0}.AvailableEngineModes references missing EngineModeId '{1}'.", ruleset.Id, engineModeId));
                    }
                }
            }
        }

        private static void ValidateTracks(DefinitionPack pack, List<string> errors)
        {
            var weatherIds = new HashSet<WeatherTypeId>(pack.WeatherTypes.Select(value => value.Id));

            foreach (var track in pack.Tracks)
            {
                if (track.LengthMeters <= 0) errors.Add(track.Id + ".LengthMeters must be > 0.");
                if (track.BaseTimeMs <= 0) errors.Add(track.Id + ".BaseTimeMs must be > 0.");
                if (track.TotalLaps <= 0) errors.Add(track.Id + ".TotalLaps must be > 0.");
                if (track.Corners < 0) errors.Add(track.Id + ".Corners must be >= 0.");

                RequireNonEmpty(track.Sectors, track.Id + ".Sectors", errors);
                ValidateBasisPoints(track.OvertakingDifficultyRatingBasisPoints, track.Id + ".OvertakingDifficultyRatingBasisPoints", errors);
                ValidateBasisPoints(track.TyreWearMultiplierBasisPoints, track.Id + ".TyreWearMultiplierBasisPoints", errors);
                if (track.FuelConsumptionKgPerLap <= 0) errors.Add(track.Id + ".FuelConsumptionKgPerLap must be > 0.");
                ValidateProbabilityMap(track.WeatherProbabilityBasisPoints, track.Id + ".WeatherProbabilityBasisPoints", errors);

                var sectorTimeSum = 0;
                for (var index = 0; index < track.Sectors.Count; index++)
                {
                    var sector = track.Sectors[index];
                    if (sector.Index != index) errors.Add(track.Id + ".Sectors must have sequential zero-based indices.");
                    if (sector.BaseTimeMs <= 0) errors.Add(track.Id + ".Sector BaseTimeMs must be > 0.");
                    ValidateBasisPoints(sector.StraightPercentBasisPoints, track.Id + ".Sector StraightPercentBasisPoints", errors);
                    if (sector.CornerCount < 0) errors.Add(track.Id + ".Sector CornerCount must be >= 0.");
                    sectorTimeSum += sector.BaseTimeMs;
                }

                if (sectorTimeSum != track.BaseTimeMs) errors.Add(track.Id + ".BaseTimeMs must equal the sum of sector BaseTimeMs.");

                var drsSectors = new HashSet<int>();
                foreach (var drsSector in track.DrsZoneSectorIndices)
                {
                    if (!drsSectors.Add(drsSector)) errors.Add(track.Id + ".DrsZoneSectorIndices must be unique.");
                    if (drsSector < 0 || drsSector >= track.Sectors.Count) errors.Add(track.Id + ".DrsZoneSectorIndices must reference valid sector indices.");
                }

                foreach (var weatherId in track.WeatherProbabilityBasisPoints.Keys)
                {
                    if (!weatherIds.Contains(weatherId))
                    {
                        errors.Add(string.Format("{0}.WeatherProbabilityBasisPoints references missing WeatherTypeId '{1}'.", track.Id, weatherId));
                    }
                }
            }
        }

        private static void ValidateSeries(DefinitionPack pack, List<string> errors)
        {
            var trackIds = new HashSet<TrackId>(pack.Tracks.Select(value => value.Id));
            var rulesetIds = new HashSet<RulesetId>(pack.Rulesets.Select(value => value.Id));

            foreach (var series in pack.Series)
            {
                if (series.Tier < 1) errors.Add(series.Id + ".Tier must be >= 1.");
                if (series.TeamCount <= 0) errors.Add(series.Id + ".TeamCount must be > 0.");
                if (series.DriversPerTeam <= 0) errors.Add(series.Id + ".DriversPerTeam must be > 0.");
                RequireNonEmpty(series.CalendarTemplate, series.Id + ".CalendarTemplate", errors);
                if (!rulesetIds.Contains(series.RulesetId)) errors.Add(string.Format("{0}.RulesetId references missing RulesetId '{1}'.", series.Id, series.RulesetId));

                foreach (var trackId in series.CalendarTemplate)
                {
                    if (!trackIds.Contains(trackId))
                    {
                        errors.Add(string.Format("{0}.CalendarTemplate references missing TrackId '{1}'.", series.Id, trackId));
                    }
                }
            }
        }

        private static void ValidatePartTypes(IReadOnlyList<PartTypeDef> partTypes, List<string> errors)
        {
            foreach (var partType in partTypes)
            {
                if (!Enum.IsDefined(typeof(PartSlot), partType.Slot)) errors.Add(partType.Id + ".Slot must be valid.");
                ValidateBasisPoints(partType.StatMinBasisPoints, partType.Id + ".StatMinBasisPoints", errors);
                ValidateBasisPoints(partType.StatMaxBasisPoints, partType.Id + ".StatMaxBasisPoints", errors);
                ValidateMinMax(partType.StatMinBasisPoints, partType.StatMaxBasisPoints, partType.Id + " stat range", errors);
                if (partType.BuildTimeBaseWeeks <= 0) errors.Add(partType.Id + ".BuildTimeBaseWeeks must be > 0.");
                if (partType.BuildCostBaseCents <= 0) errors.Add(partType.Id + ".BuildCostBaseCents must be > 0.");
                if (partType.WeightGrams < 0) errors.Add(partType.Id + ".WeightGrams must be >= 0.");
            }
        }

        private static void ValidateSponsors(IReadOnlyList<SponsorDef> sponsors, List<string> errors)
        {
            foreach (var sponsor in sponsors)
            {
                if (!Enum.IsDefined(typeof(SponsorTier), sponsor.Tier)) errors.Add(sponsor.Id + ".Tier must be valid.");
                if (sponsor.PaymentPerRaceCents < 0) errors.Add(sponsor.Id + ".PaymentPerRaceCents must be >= 0.");
                if (sponsor.SigningBonusCents < 0) errors.Add(sponsor.Id + ".SigningBonusCents must be >= 0.");
                ValidateBasisPoints(sponsor.MinTeamReputationBasisPoints, sponsor.Id + ".MinTeamReputationBasisPoints", errors);
                if (sponsor.MinChampionshipPosition < 0) errors.Add(sponsor.Id + ".MinChampionshipPosition must be >= 0.");
                RequireNonEmpty(sponsor.ObjectiveTemplates, sponsor.Id + ".ObjectiveTemplates", errors);
                if (sponsor.ObjectiveTemplates.Any(objective => objective.PenaltyCents < 0)) errors.Add(sponsor.Id + ".ObjectiveTemplates PenaltyCents must be >= 0.");
            }
        }

        private static void ValidateBuildings(IReadOnlyList<BuildingDef> buildings, List<string> errors)
        {
            foreach (var building in buildings)
            {
                RequireNonEmpty(building.Levels, building.Id + ".Levels", errors);
                for (var index = 0; index < building.Levels.Count; index++)
                {
                    var level = building.Levels[index];
                    if (level.Level != index + 1) errors.Add(building.Id + ".Levels must be contiguous and start at 1.");
                    if (level.UpgradeCostCents < 0) errors.Add(building.Id + ".UpgradeCostCents must be >= 0.");
                    if (level.UpgradeTimeWeeks <= 0) errors.Add(building.Id + ".UpgradeTimeWeeks must be > 0.");
                    RequireNonEmpty(level.Effects, building.Id + ".Effects", errors);

                    foreach (var effect in level.Effects)
                    {
                        if (!Enum.IsDefined(typeof(BuildingEffectType), effect.EffectType)) errors.Add(building.Id + ".EffectType must be valid.");
                        ValidateBasisPoints(effect.ValueBasisPoints, building.Id + ".Effect ValueBasisPoints", errors);
                    }
                }
            }
        }

        private static void ValidateComponents(IReadOnlyList<ComponentDef> components, List<string> errors)
        {
            foreach (var component in components)
            {
                if (!Enum.IsDefined(typeof(PartSlot), component.TargetSlot)) errors.Add(component.Id + ".TargetSlot must be valid.");
                ValidateBasisPoints(component.StatContributionMinBasisPoints, component.Id + ".StatContributionMinBasisPoints", errors);
                ValidateBasisPoints(component.StatContributionMaxBasisPoints, component.Id + ".StatContributionMaxBasisPoints", errors);
                ValidateBasisPoints(component.ReliabilityContributionMinBasisPoints, component.Id + ".ReliabilityContributionMinBasisPoints", errors);
                ValidateBasisPoints(component.ReliabilityContributionMaxBasisPoints, component.Id + ".ReliabilityContributionMaxBasisPoints", errors);
                ValidateMinMax(component.StatContributionMinBasisPoints, component.StatContributionMaxBasisPoints, component.Id + " stat contribution range", errors);
                ValidateMinMax(component.ReliabilityContributionMinBasisPoints, component.ReliabilityContributionMaxBasisPoints, component.Id + " reliability contribution range", errors);
            }
        }

        private static void ValidateDriverArchetypes(IReadOnlyList<DriverArchetypeDef> archetypes, List<string> errors)
        {
            foreach (var archetype in archetypes)
            {
                ValidateBasisPoints(archetype.SpeedMinBasisPoints, archetype.Id + ".SpeedMinBasisPoints", errors);
                ValidateBasisPoints(archetype.SpeedMaxBasisPoints, archetype.Id + ".SpeedMaxBasisPoints", errors);
                ValidateBasisPoints(archetype.ConsistencyMinBasisPoints, archetype.Id + ".ConsistencyMinBasisPoints", errors);
                ValidateBasisPoints(archetype.ConsistencyMaxBasisPoints, archetype.Id + ".ConsistencyMaxBasisPoints", errors);
                ValidateBasisPoints(archetype.WetSkillMinBasisPoints, archetype.Id + ".WetSkillMinBasisPoints", errors);
                ValidateBasisPoints(archetype.WetSkillMaxBasisPoints, archetype.Id + ".WetSkillMaxBasisPoints", errors);
                ValidateBasisPoints(archetype.OvertakingMinBasisPoints, archetype.Id + ".OvertakingMinBasisPoints", errors);
                ValidateBasisPoints(archetype.OvertakingMaxBasisPoints, archetype.Id + ".OvertakingMaxBasisPoints", errors);
                ValidateBasisPoints(archetype.DefenceMinBasisPoints, archetype.Id + ".DefenceMinBasisPoints", errors);
                ValidateBasisPoints(archetype.DefenceMaxBasisPoints, archetype.Id + ".DefenceMaxBasisPoints", errors);
                ValidateBasisPoints(archetype.StartingReputationMinBasisPoints, archetype.Id + ".StartingReputationMinBasisPoints", errors);
                ValidateBasisPoints(archetype.StartingReputationMaxBasisPoints, archetype.Id + ".StartingReputationMaxBasisPoints", errors);
                ValidateMinMax(archetype.SpeedMinBasisPoints, archetype.SpeedMaxBasisPoints, archetype.Id + " speed range", errors);
                ValidateMinMax(archetype.ConsistencyMinBasisPoints, archetype.ConsistencyMaxBasisPoints, archetype.Id + " consistency range", errors);
                ValidateMinMax(archetype.WetSkillMinBasisPoints, archetype.WetSkillMaxBasisPoints, archetype.Id + " wet skill range", errors);
                ValidateMinMax(archetype.OvertakingMinBasisPoints, archetype.OvertakingMaxBasisPoints, archetype.Id + " overtaking range", errors);
                ValidateMinMax(archetype.DefenceMinBasisPoints, archetype.DefenceMaxBasisPoints, archetype.Id + " defence range", errors);
                ValidateMinMax(archetype.AgeMin, archetype.AgeMax, archetype.Id + " age range", errors);
                ValidateMinMax(archetype.StartingReputationMinBasisPoints, archetype.StartingReputationMaxBasisPoints, archetype.Id + " starting reputation range", errors);
                if (archetype.AgeMin <= 0 || archetype.AgeMax <= 0) errors.Add(archetype.Id + ".AgeMin and AgeMax must be > 0.");
            }
        }

        private static void ValidateTypeShape(List<string> errors)
        {
            var types = new[]
            {
                typeof(TyreCompoundDef), typeof(WeatherDef), typeof(SetupPenaltyEntry), typeof(EngineModeScalar),
                typeof(PartRankTimeCostTable), typeof(DriverPaceScalars), typeof(SetupScalars), typeof(KnowledgeScalars),
                typeof(TyreTemperatureScalars), typeof(FuelScalars), typeof(DraftingScalars), typeof(VarianceScalars),
                typeof(ScalarTables), typeof(SectorDef), typeof(SeriesDef), typeof(RulesetDef), typeof(TrackDef),
                typeof(PartTypeDef), typeof(SponsorObjectiveTemplate), typeof(SponsorDef), typeof(BuildingEffectDef),
                typeof(BuildingLevelDef), typeof(BuildingDef), typeof(ComponentDef), typeof(DriverArchetypeDef),
            };

            foreach (var type in types)
            {
                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (ContainsFloatOrDouble(property.PropertyType)) errors.Add(type.Name + "." + property.Name + " must not use float or double.");
                }

                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (ContainsFloatOrDouble(field.FieldType)) errors.Add(type.Name + "." + field.Name + " must not use float or double.");
                }
            }
        }

        private static bool ContainsFloatOrDouble(Type type)
        {
            if (type == typeof(float) || type == typeof(double)) return true;
            if (type.IsArray) return ContainsFloatOrDouble(type.GetElementType());
            return type.IsGenericType && type.GetGenericArguments().Any(ContainsFloatOrDouble);
        }

        private static void ValidateUniqueIds<TDefinition, TId>(IEnumerable<TDefinition> definitions, Func<TDefinition, TId> idSelector, string typeName, List<string> errors)
        {
            var seen = new HashSet<TId>();
            foreach (var definition in definitions)
            {
                var id = idSelector(definition);
                if (!seen.Add(id))
                {
                    errors.Add(string.Format("Duplicate {0} id '{1}'.", typeName, id));
                }
            }
        }

        private static void ValidateProbabilityMap<TId>(IReadOnlyDictionary<TId, int> values, string name, List<string> errors)
        {
            if (values == null || values.Count == 0)
            {
                errors.Add(name + " must be non-empty.");
                return;
            }

            var sum = 0;
            foreach (var value in values.Values)
            {
                if (value < 0) errors.Add(name + " values must be >= 0.");
                sum += value;
            }

            if (sum != 10000) errors.Add(name + " must sum to exactly 10000.");
        }

        private static void ValidateBasisPoints(int value, string name, List<string> errors) => ValidateRangeInclusive(value, 0, 10000, name, errors);

        private static void ValidateRangeInclusive(int value, int min, int max, string name, List<string> errors)
        {
            if (value < min || value > max)
            {
                errors.Add(string.Format("{0} must be between {1} and {2}.", name, min, max));
            }
        }

        private static void ValidateMinMax(int min, int max, string name, List<string> errors)
        {
            if (min > max) errors.Add(name + " must satisfy min <= max.");
        }

        private static void RequireNonEmpty<T>(IReadOnlyCollection<T> values, string name, List<string> errors)
        {
            if (values == null || values.Count == 0) errors.Add(name + " must be non-empty.");
        }
    }
}
