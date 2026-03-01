namespace MotorracingManager.Core.Enums
{
    public enum PartSlot
    {
        Engine = 0,
        Aero = 1,
        Chassis = 2,
    }

    public enum SponsorTier
    {
        Title = 0,
        Major = 1,
        Minor = 2,
        Technical = 3,
    }

    public enum WeatherState
    {
        Dry = 0,
        Damp = 1,
        Wet = 2,
        Monsoon = 3,
    }

    public enum TyrePhase
    {
        Fresh = 0,
        Optimal = 1,
        Worn = 2,
        Cliff = 3,
    }

    public enum SessionType
    {
        Practice1 = 0,
        Practice2 = 1,
        Practice3 = 2,
        Qualifying1 = 3,
        Qualifying2 = 4,
        Qualifying3 = 5,
        SprintQualifying = 6,
        Sprint = 7,
        Race = 8,
    }

    public enum QualifyingFormat
    {
        SingleSession = 0,
        ThreeKnockout = 1,
    }

    public enum BuildingEffectType
    {
        DevelopmentSpeed = 0,
        ReliabilityBonus = 1,
        PitStopSpeed = 2,
        SetupAccuracy = 3,
        KnowledgeRetention = 4,
    }
}
