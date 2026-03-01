using System.Collections.Generic;
using MotorracingManager.Core.Definitions;
using MotorracingManager.Core.Definitions.Scalars;
using MotorracingManager.Core.Identifiers;

namespace MotorracingManager.Domain.Definitions
{
    public interface IDefinitionRegistry
    {
        SeriesDef GetSeries(SeriesId id);
        RulesetDef GetRuleset(RulesetId id);
        TrackDef GetTrack(TrackId id);
        PartTypeDef GetPartType(PartTypeId id);
        TyreCompoundDef GetTyreCompound(TyreCompoundId id);
        SponsorDef GetSponsor(SponsorId id);
        BuildingDef GetBuilding(BuildingId id);
        ComponentDef GetComponent(ComponentId id);
        DriverArchetypeDef GetDriverArchetype(DriverArchetypeId id);
        WeatherDef GetWeatherType(WeatherTypeId id);

        IReadOnlyList<SeriesDef> AllSeries { get; }
        IReadOnlyList<RulesetDef> AllRulesets { get; }
        IReadOnlyList<TrackDef> AllTracks { get; }
        IReadOnlyList<PartTypeDef> AllPartTypes { get; }
        IReadOnlyList<TyreCompoundDef> AllTyreCompounds { get; }
        IReadOnlyList<SponsorDef> AllSponsors { get; }
        IReadOnlyList<BuildingDef> AllBuildings { get; }
        IReadOnlyList<ComponentDef> AllComponents { get; }
        IReadOnlyList<DriverArchetypeDef> AllDriverArchetypes { get; }
        IReadOnlyList<WeatherDef> AllWeatherTypes { get; }
        ScalarTables Scalars { get; }
    }

    public interface IDefinitionValidator
    {
        ValidationResult Validate(DefinitionPack pack);
    }

    public interface IDefinitionPackLoader
    {
        DefinitionPack LoadFromDirectory(string directoryPath);
    }
}
