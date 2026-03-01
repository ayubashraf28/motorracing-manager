using System.Collections.Generic;
using MotorracingManager.Core.Enums;
using MotorracingManager.Core.Identifiers;

namespace MotorracingManager.Domain.Definitions
{
    public readonly struct SponsorObjectiveTemplate
    {
        public SponsorObjectiveTemplate(string description, int targetValue, string metricKey, long penaltyCents)
        {
            Description = Guard.AgainstNullOrWhiteSpace(description, nameof(description));
            TargetValue = targetValue;
            MetricKey = Guard.AgainstNullOrWhiteSpace(metricKey, nameof(metricKey));
            PenaltyCents = penaltyCents;
        }

        public string Description { get; }
        public int TargetValue { get; }
        public string MetricKey { get; }
        public long PenaltyCents { get; }
    }

    public sealed class SponsorDef
    {
        public SponsorDef(
            SponsorId id,
            string displayName,
            SponsorTier tier,
            long paymentPerRaceCents,
            long signingBonusCents,
            int minTeamReputationBasisPoints,
            int minChampionshipPosition,
            IReadOnlyList<SponsorObjectiveTemplate> objectiveTemplates)
        {
            Guard.AgainstDefaultIdentifier(id.Value, nameof(id));
            Id = id;
            DisplayName = Guard.AgainstNullOrWhiteSpace(displayName, nameof(displayName));
            Tier = tier;
            PaymentPerRaceCents = paymentPerRaceCents;
            SigningBonusCents = signingBonusCents;
            MinTeamReputationBasisPoints = minTeamReputationBasisPoints;
            MinChampionshipPosition = minChampionshipPosition;
            ObjectiveTemplates = Guard.AgainstNullOrCopy(objectiveTemplates, nameof(objectiveTemplates));
        }

        public SponsorId Id { get; }
        public string DisplayName { get; }
        public SponsorTier Tier { get; }
        public long PaymentPerRaceCents { get; }
        public long SigningBonusCents { get; }
        public int MinTeamReputationBasisPoints { get; }
        public int MinChampionshipPosition { get; }
        public IReadOnlyList<SponsorObjectiveTemplate> ObjectiveTemplates { get; }
    }

    public readonly struct BuildingEffectDef
    {
        public BuildingEffectDef(BuildingEffectType effectType, int valueBasisPoints)
        {
            EffectType = effectType;
            ValueBasisPoints = valueBasisPoints;
        }

        public BuildingEffectType EffectType { get; }
        public int ValueBasisPoints { get; }
    }

    public sealed class BuildingLevelDef
    {
        public BuildingLevelDef(int level, long upgradeCostCents, int upgradeTimeWeeks, IReadOnlyList<BuildingEffectDef> effects)
        {
            Level = level;
            UpgradeCostCents = upgradeCostCents;
            UpgradeTimeWeeks = upgradeTimeWeeks;
            Effects = Guard.AgainstNullOrCopy(effects, nameof(effects));
        }

        public int Level { get; }
        public long UpgradeCostCents { get; }
        public int UpgradeTimeWeeks { get; }
        public IReadOnlyList<BuildingEffectDef> Effects { get; }
    }

    public sealed class BuildingDef
    {
        public BuildingDef(BuildingId id, string displayName, IReadOnlyList<BuildingLevelDef> levels)
        {
            Guard.AgainstDefaultIdentifier(id.Value, nameof(id));
            Id = id;
            DisplayName = Guard.AgainstNullOrWhiteSpace(displayName, nameof(displayName));
            Levels = Guard.AgainstNullOrCopy(levels, nameof(levels));
        }

        public BuildingId Id { get; }
        public string DisplayName { get; }
        public IReadOnlyList<BuildingLevelDef> Levels { get; }
    }
}
