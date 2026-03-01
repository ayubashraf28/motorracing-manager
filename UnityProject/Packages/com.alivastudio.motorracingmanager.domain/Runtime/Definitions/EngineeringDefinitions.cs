using MotorracingManager.Core.Enums;
using MotorracingManager.Core.Identifiers;

namespace MotorracingManager.Domain.Definitions
{
    public sealed class PartTypeDef
    {
        public PartTypeDef(
            PartTypeId id,
            string displayName,
            PartSlot slot,
            int statMinBasisPoints,
            int statMaxBasisPoints,
            int buildTimeBaseWeeks,
            long buildCostBaseCents,
            int weightGrams)
        {
            Guard.AgainstDefaultIdentifier(id.Value, nameof(id));
            Id = id;
            DisplayName = Guard.AgainstNullOrWhiteSpace(displayName, nameof(displayName));
            Slot = slot;
            StatMinBasisPoints = statMinBasisPoints;
            StatMaxBasisPoints = statMaxBasisPoints;
            BuildTimeBaseWeeks = buildTimeBaseWeeks;
            BuildCostBaseCents = buildCostBaseCents;
            WeightGrams = weightGrams;
        }

        public PartTypeId Id { get; }
        public string DisplayName { get; }
        public PartSlot Slot { get; }
        public int StatMinBasisPoints { get; }
        public int StatMaxBasisPoints { get; }
        public int BuildTimeBaseWeeks { get; }
        public long BuildCostBaseCents { get; }
        public int WeightGrams { get; }
    }

    public sealed class ComponentDef
    {
        public ComponentDef(
            ComponentId id,
            string displayName,
            PartSlot targetSlot,
            int statContributionMinBasisPoints,
            int statContributionMaxBasisPoints,
            int reliabilityContributionMinBasisPoints,
            int reliabilityContributionMaxBasisPoints,
            int buildTimeModifierWeeks,
            long costModifierCents)
        {
            Guard.AgainstDefaultIdentifier(id.Value, nameof(id));
            Id = id;
            DisplayName = Guard.AgainstNullOrWhiteSpace(displayName, nameof(displayName));
            TargetSlot = targetSlot;
            StatContributionMinBasisPoints = statContributionMinBasisPoints;
            StatContributionMaxBasisPoints = statContributionMaxBasisPoints;
            ReliabilityContributionMinBasisPoints = reliabilityContributionMinBasisPoints;
            ReliabilityContributionMaxBasisPoints = reliabilityContributionMaxBasisPoints;
            BuildTimeModifierWeeks = buildTimeModifierWeeks;
            CostModifierCents = costModifierCents;
        }

        public ComponentId Id { get; }
        public string DisplayName { get; }
        public PartSlot TargetSlot { get; }
        public int StatContributionMinBasisPoints { get; }
        public int StatContributionMaxBasisPoints { get; }
        public int ReliabilityContributionMinBasisPoints { get; }
        public int ReliabilityContributionMaxBasisPoints { get; }
        public int BuildTimeModifierWeeks { get; }
        public long CostModifierCents { get; }
    }
}
