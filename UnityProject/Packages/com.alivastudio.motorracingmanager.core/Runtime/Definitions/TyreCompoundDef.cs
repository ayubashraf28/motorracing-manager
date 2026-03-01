using MotorracingManager.Core.Identifiers;

namespace MotorracingManager.Core.Definitions
{
    public sealed class TyreCompoundDef
    {
        public TyreCompoundDef(
            TyreCompoundId id,
            string displayName,
            int gripRatingBasisPoints,
            int wearRatePerLapBasisPoints,
            int freshLaps,
            int optimalLaps,
            int wornLaps,
            int cliffMsPerLap,
            int optimalBonusMs,
            int freshPenaltyMs,
            int temperatureWindowLowCelsius,
            int temperatureWindowHighCelsius,
            int warmupRateCelsiusPerLap)
        {
            Id = RequireIdentifier(id, nameof(id));
            DisplayName = Guard.AgainstNullOrWhiteSpace(displayName, nameof(displayName));
            GripRatingBasisPoints = gripRatingBasisPoints;
            WearRatePerLapBasisPoints = wearRatePerLapBasisPoints;
            FreshLaps = freshLaps;
            OptimalLaps = optimalLaps;
            WornLaps = wornLaps;
            CliffMsPerLap = cliffMsPerLap;
            OptimalBonusMs = optimalBonusMs;
            FreshPenaltyMs = freshPenaltyMs;
            TemperatureWindowLowCelsius = temperatureWindowLowCelsius;
            TemperatureWindowHighCelsius = temperatureWindowHighCelsius;
            WarmupRateCelsiusPerLap = warmupRateCelsiusPerLap;
        }

        public TyreCompoundId Id { get; }
        public string DisplayName { get; }
        public int GripRatingBasisPoints { get; }
        public int WearRatePerLapBasisPoints { get; }
        public int FreshLaps { get; }
        public int OptimalLaps { get; }
        public int WornLaps { get; }
        public int CliffMsPerLap { get; }
        public int OptimalBonusMs { get; }
        public int FreshPenaltyMs { get; }
        public int TemperatureWindowLowCelsius { get; }
        public int TemperatureWindowHighCelsius { get; }
        public int WarmupRateCelsiusPerLap { get; }

        private static TyreCompoundId RequireIdentifier(TyreCompoundId id, string paramName)
        {
            if (string.IsNullOrWhiteSpace(id.Value))
            {
                throw new System.ArgumentException("Identifier cannot be default or whitespace.", paramName);
            }

            return id;
        }
    }
}
