using System.Collections.Generic;
using MotorracingManager.Core.Identifiers;

namespace MotorracingManager.Domain.Definitions
{
    public readonly struct SectorDef
    {
        public SectorDef(int index, int baseTimeMs, int straightPercentBasisPoints, int cornerCount)
        {
            Index = index;
            BaseTimeMs = baseTimeMs;
            StraightPercentBasisPoints = straightPercentBasisPoints;
            CornerCount = cornerCount;
        }

        public int Index { get; }
        public int BaseTimeMs { get; }
        public int StraightPercentBasisPoints { get; }
        public int CornerCount { get; }
    }

    public sealed class TrackDef
    {
        public TrackDef(
            TrackId id,
            string displayName,
            string country,
            int lengthMeters,
            int baseTimeMs,
            int totalLaps,
            int corners,
            IReadOnlyList<SectorDef> sectors,
            int overtakingDifficultyRatingBasisPoints,
            int tyreWearMultiplierBasisPoints,
            int fuelConsumptionKgPerLap,
            IReadOnlyDictionary<WeatherTypeId, int> weatherProbabilityBasisPoints,
            int ambientTemperatureBaseCelsius,
            int trackTemperatureBaseCelsius,
            IReadOnlyList<int> drsZoneSectorIndices)
        {
            Guard.AgainstDefaultIdentifier(id.Value, nameof(id));
            Id = id;
            DisplayName = Guard.AgainstNullOrWhiteSpace(displayName, nameof(displayName));
            Country = Guard.AgainstNullOrWhiteSpace(country, nameof(country));
            LengthMeters = lengthMeters;
            BaseTimeMs = baseTimeMs;
            TotalLaps = totalLaps;
            Corners = corners;
            Sectors = Guard.AgainstNullOrCopy(sectors, nameof(sectors));
            OvertakingDifficultyRatingBasisPoints = overtakingDifficultyRatingBasisPoints;
            TyreWearMultiplierBasisPoints = tyreWearMultiplierBasisPoints;
            FuelConsumptionKgPerLap = fuelConsumptionKgPerLap;
            WeatherProbabilityBasisPoints = Guard.AgainstNullOrCopyDictionary(weatherProbabilityBasisPoints, nameof(weatherProbabilityBasisPoints));
            AmbientTemperatureBaseCelsius = ambientTemperatureBaseCelsius;
            TrackTemperatureBaseCelsius = trackTemperatureBaseCelsius;
            DrsZoneSectorIndices = Guard.AgainstNullOrCopy(drsZoneSectorIndices, nameof(drsZoneSectorIndices));
        }

        public TrackId Id { get; }
        public string DisplayName { get; }
        public string Country { get; }
        public int LengthMeters { get; }
        public int BaseTimeMs { get; }
        public int TotalLaps { get; }
        public int Corners { get; }
        public IReadOnlyList<SectorDef> Sectors { get; }
        public int OvertakingDifficultyRatingBasisPoints { get; }
        public int TyreWearMultiplierBasisPoints { get; }
        public int FuelConsumptionKgPerLap { get; }
        public IReadOnlyDictionary<WeatherTypeId, int> WeatherProbabilityBasisPoints { get; }
        public int AmbientTemperatureBaseCelsius { get; }
        public int TrackTemperatureBaseCelsius { get; }
        public IReadOnlyList<int> DrsZoneSectorIndices { get; }
    }
}
