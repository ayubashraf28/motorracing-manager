using System;
using System.Collections.Generic;
using MotorracingManager.Core.Enums;
using MotorracingManager.Core.Identifiers;

namespace MotorracingManager.Core.Definitions.Scalars
{
    public readonly struct SetupPenaltyEntry
    {
        public SetupPenaltyEntry(int matchPercentage, int penaltyMs)
        {
            MatchPercentage = matchPercentage;
            PenaltyMs = penaltyMs;
        }

        public int MatchPercentage { get; }
        public int PenaltyMs { get; }
    }

    public sealed class EngineModeScalar
    {
        public EngineModeScalar(
            EngineModeId id,
            int paceModifierMs,
            int reliabilityMultiplierBasisPoints,
            int fuelEfficiencyMultiplierBasisPoints)
        {
            if (string.IsNullOrWhiteSpace(id.Value))
            {
                throw new ArgumentException("Identifier cannot be default or whitespace.", nameof(id));
            }

            Id = id;
            PaceModifierMs = paceModifierMs;
            ReliabilityMultiplierBasisPoints = reliabilityMultiplierBasisPoints;
            FuelEfficiencyMultiplierBasisPoints = fuelEfficiencyMultiplierBasisPoints;
        }

        public EngineModeId Id { get; }
        public int PaceModifierMs { get; }
        public int ReliabilityMultiplierBasisPoints { get; }
        public int FuelEfficiencyMultiplierBasisPoints { get; }
    }

    public sealed class PartRankTimeCostTable
    {
        public PartRankTimeCostTable(IReadOnlyDictionary<PartSlot, IReadOnlyList<int>> rankToMsByCategory)
        {
            if (rankToMsByCategory == null)
            {
                throw new ArgumentNullException(nameof(rankToMsByCategory));
            }

            var copy = new Dictionary<PartSlot, IReadOnlyList<int>>();
            foreach (var pair in rankToMsByCategory)
            {
                copy[pair.Key] = Guard.AgainstNullOrCopy(pair.Value, nameof(rankToMsByCategory));
            }

            RankToMsByCategory = copy;
        }

        public IReadOnlyDictionary<PartSlot, IReadOnlyList<int>> RankToMsByCategory { get; }

        public int LookupMs(PartSlot slot, int rank)
        {
            var values = RankToMsByCategory[slot];
            var index = rank <= 1 ? 0 : rank - 1;
            if (index >= values.Count)
            {
                index = values.Count - 1;
            }

            return values[index];
        }
    }

    public sealed class DriverPaceScalars
    {
        public DriverPaceScalars(int baselineRating, int msPerRatingPoint)
        {
            BaselineRating = baselineRating;
            MsPerRatingPoint = msPerRatingPoint;
        }

        public int BaselineRating { get; }
        public int MsPerRatingPoint { get; }
    }

    public sealed class SetupScalars
    {
        public SetupScalars(IReadOnlyList<SetupPenaltyEntry> penalties)
        {
            Penalties = Guard.AgainstNullOrCopy(penalties, nameof(penalties));
        }

        public IReadOnlyList<SetupPenaltyEntry> Penalties { get; }
    }

    public sealed class KnowledgeScalars
    {
        public KnowledgeScalars(int maxPenaltyMs, int retentionPercentPerSeason)
        {
            MaxPenaltyMs = maxPenaltyMs;
            RetentionPercentPerSeason = retentionPercentPerSeason;
        }

        public int MaxPenaltyMs { get; }
        public int RetentionPercentPerSeason { get; }
    }

    public sealed class TyreTemperatureScalars
    {
        public TyreTemperatureScalars(int msPerDegreeOutsideWindow)
        {
            MsPerDegreeOutsideWindow = msPerDegreeOutsideWindow;
        }

        public int MsPerDegreeOutsideWindow { get; }
    }

    public sealed class FuelScalars
    {
        public FuelScalars(int msPerKg)
        {
            MsPerKg = msPerKg;
        }

        public int MsPerKg { get; }
    }

    public sealed class DraftingScalars
    {
        public DraftingScalars(int maxBenefitMs, int thresholdGapMs, int drsBonusMs)
        {
            MaxBenefitMs = maxBenefitMs;
            ThresholdGapMs = thresholdGapMs;
            DrsBonusMs = drsBonusMs;
        }

        public int MaxBenefitMs { get; }
        public int ThresholdGapMs { get; }
        public int DrsBonusMs { get; }
    }

    public sealed class VarianceScalars
    {
        public VarianceScalars(int baseRangeMs, int consistencyAttenuationBasisPoints)
        {
            BaseRangeMs = baseRangeMs;
            ConsistencyAttenuationBasisPoints = consistencyAttenuationBasisPoints;
        }

        public int BaseRangeMs { get; }
        public int ConsistencyAttenuationBasisPoints { get; }
    }

    public sealed class ScalarTables
    {
        public ScalarTables(
            PartRankTimeCostTable partRankTimeCostTable,
            DriverPaceScalars driverPace,
            SetupScalars setup,
            KnowledgeScalars knowledge,
            TyreTemperatureScalars tyreTemperature,
            FuelScalars fuel,
            IReadOnlyList<EngineModeScalar> engineModes,
            DraftingScalars drafting,
            VarianceScalars variance)
        {
            PartRankTimeCostTable = partRankTimeCostTable ?? throw new ArgumentNullException(nameof(partRankTimeCostTable));
            DriverPace = driverPace ?? throw new ArgumentNullException(nameof(driverPace));
            Setup = setup ?? throw new ArgumentNullException(nameof(setup));
            Knowledge = knowledge ?? throw new ArgumentNullException(nameof(knowledge));
            TyreTemperature = tyreTemperature ?? throw new ArgumentNullException(nameof(tyreTemperature));
            Fuel = fuel ?? throw new ArgumentNullException(nameof(fuel));
            EngineModes = Guard.AgainstNullOrCopy(engineModes, nameof(engineModes));
            Drafting = drafting ?? throw new ArgumentNullException(nameof(drafting));
            Variance = variance ?? throw new ArgumentNullException(nameof(variance));
        }

        public PartRankTimeCostTable PartRankTimeCostTable { get; }
        public DriverPaceScalars DriverPace { get; }
        public SetupScalars Setup { get; }
        public KnowledgeScalars Knowledge { get; }
        public TyreTemperatureScalars TyreTemperature { get; }
        public FuelScalars Fuel { get; }
        public IReadOnlyList<EngineModeScalar> EngineModes { get; }
        public DraftingScalars Drafting { get; }
        public VarianceScalars Variance { get; }
    }
}
