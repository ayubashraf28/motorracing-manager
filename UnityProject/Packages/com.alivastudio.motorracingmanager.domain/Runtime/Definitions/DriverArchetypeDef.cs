using MotorracingManager.Core.Identifiers;

namespace MotorracingManager.Domain.Definitions
{
    public sealed class DriverArchetypeDef
    {
        public DriverArchetypeDef(
            DriverArchetypeId id,
            string displayName,
            int speedMinBasisPoints,
            int speedMaxBasisPoints,
            int consistencyMinBasisPoints,
            int consistencyMaxBasisPoints,
            int wetSkillMinBasisPoints,
            int wetSkillMaxBasisPoints,
            int overtakingMinBasisPoints,
            int overtakingMaxBasisPoints,
            int defenceMinBasisPoints,
            int defenceMaxBasisPoints,
            int ageMin,
            int ageMax,
            int startingReputationMinBasisPoints,
            int startingReputationMaxBasisPoints)
        {
            Guard.AgainstDefaultIdentifier(id.Value, nameof(id));
            Id = id;
            DisplayName = Guard.AgainstNullOrWhiteSpace(displayName, nameof(displayName));
            SpeedMinBasisPoints = speedMinBasisPoints;
            SpeedMaxBasisPoints = speedMaxBasisPoints;
            ConsistencyMinBasisPoints = consistencyMinBasisPoints;
            ConsistencyMaxBasisPoints = consistencyMaxBasisPoints;
            WetSkillMinBasisPoints = wetSkillMinBasisPoints;
            WetSkillMaxBasisPoints = wetSkillMaxBasisPoints;
            OvertakingMinBasisPoints = overtakingMinBasisPoints;
            OvertakingMaxBasisPoints = overtakingMaxBasisPoints;
            DefenceMinBasisPoints = defenceMinBasisPoints;
            DefenceMaxBasisPoints = defenceMaxBasisPoints;
            AgeMin = ageMin;
            AgeMax = ageMax;
            StartingReputationMinBasisPoints = startingReputationMinBasisPoints;
            StartingReputationMaxBasisPoints = startingReputationMaxBasisPoints;
        }

        public DriverArchetypeId Id { get; }
        public string DisplayName { get; }
        public int SpeedMinBasisPoints { get; }
        public int SpeedMaxBasisPoints { get; }
        public int ConsistencyMinBasisPoints { get; }
        public int ConsistencyMaxBasisPoints { get; }
        public int WetSkillMinBasisPoints { get; }
        public int WetSkillMaxBasisPoints { get; }
        public int OvertakingMinBasisPoints { get; }
        public int OvertakingMaxBasisPoints { get; }
        public int DefenceMinBasisPoints { get; }
        public int DefenceMaxBasisPoints { get; }
        public int AgeMin { get; }
        public int AgeMax { get; }
        public int StartingReputationMinBasisPoints { get; }
        public int StartingReputationMaxBasisPoints { get; }
    }
}
