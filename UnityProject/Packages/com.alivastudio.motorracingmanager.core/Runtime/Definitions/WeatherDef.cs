using System.Collections.Generic;
using MotorracingManager.Core.Enums;
using MotorracingManager.Core.Identifiers;

namespace MotorracingManager.Core.Definitions
{
    public sealed class WeatherDef
    {
        public WeatherDef(
            WeatherTypeId id,
            string displayName,
            WeatherState state,
            int basePenaltyMs,
            int gripModifierBasisPoints,
            IReadOnlyDictionary<TyreCompoundId, int> compoundMatchPenaltyMs,
            IReadOnlyDictionary<WeatherTypeId, int> transitionProbabilityBasisPoints,
            int trackTemperatureDeltaCelsius)
        {
            Id = RequireIdentifier(id, nameof(id));
            DisplayName = Guard.AgainstNullOrWhiteSpace(displayName, nameof(displayName));
            State = state;
            BasePenaltyMs = basePenaltyMs;
            GripModifierBasisPoints = gripModifierBasisPoints;
            CompoundMatchPenaltyMs = Guard.AgainstNullOrCopyDictionary(compoundMatchPenaltyMs, nameof(compoundMatchPenaltyMs));
            TransitionProbabilityBasisPoints = Guard.AgainstNullOrCopyDictionary(transitionProbabilityBasisPoints, nameof(transitionProbabilityBasisPoints));
            TrackTemperatureDeltaCelsius = trackTemperatureDeltaCelsius;
        }

        public WeatherTypeId Id { get; }
        public string DisplayName { get; }
        public WeatherState State { get; }
        public int BasePenaltyMs { get; }
        public int GripModifierBasisPoints { get; }
        public IReadOnlyDictionary<TyreCompoundId, int> CompoundMatchPenaltyMs { get; }
        public IReadOnlyDictionary<WeatherTypeId, int> TransitionProbabilityBasisPoints { get; }
        public int TrackTemperatureDeltaCelsius { get; }

        private static WeatherTypeId RequireIdentifier(WeatherTypeId id, string paramName)
        {
            if (string.IsNullOrWhiteSpace(id.Value))
            {
                throw new System.ArgumentException("Identifier cannot be default or whitespace.", paramName);
            }

            return id;
        }
    }
}
