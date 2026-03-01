using System;
using System.Collections.Generic;
using MotorracingManager.Core.Definitions;
using MotorracingManager.Core.Definitions.Scalars;
using MotorracingManager.Core.Identifiers;

namespace MotorracingManager.Domain.Definitions
{
    public sealed class DefinitionRegistry : IDefinitionRegistry
    {
        private readonly Dictionary<SeriesId, SeriesDef> _seriesById;
        private readonly Dictionary<RulesetId, RulesetDef> _rulesetsById;
        private readonly Dictionary<TrackId, TrackDef> _tracksById;
        private readonly Dictionary<PartTypeId, PartTypeDef> _partTypesById;
        private readonly Dictionary<TyreCompoundId, TyreCompoundDef> _tyreCompoundsById;
        private readonly Dictionary<SponsorId, SponsorDef> _sponsorsById;
        private readonly Dictionary<BuildingId, BuildingDef> _buildingsById;
        private readonly Dictionary<ComponentId, ComponentDef> _componentsById;
        private readonly Dictionary<DriverArchetypeId, DriverArchetypeDef> _driverArchetypesById;
        private readonly Dictionary<WeatherTypeId, WeatherDef> _weatherTypesById;

        public DefinitionRegistry(DefinitionPack pack)
        {
            if (pack == null)
            {
                throw new ArgumentNullException(nameof(pack));
            }

            AllSeries = pack.Series;
            AllRulesets = pack.Rulesets;
            AllTracks = pack.Tracks;
            AllPartTypes = pack.PartTypes;
            AllTyreCompounds = pack.TyreCompounds;
            AllSponsors = pack.Sponsors;
            AllBuildings = pack.Buildings;
            AllComponents = pack.Components;
            AllDriverArchetypes = pack.DriverArchetypes;
            AllWeatherTypes = pack.WeatherTypes;
            Scalars = pack.Scalars;

            _seriesById = BuildDictionary(pack.Series, definition => definition.Id);
            _rulesetsById = BuildDictionary(pack.Rulesets, definition => definition.Id);
            _tracksById = BuildDictionary(pack.Tracks, definition => definition.Id);
            _partTypesById = BuildDictionary(pack.PartTypes, definition => definition.Id);
            _tyreCompoundsById = BuildDictionary(pack.TyreCompounds, definition => definition.Id);
            _sponsorsById = BuildDictionary(pack.Sponsors, definition => definition.Id);
            _buildingsById = BuildDictionary(pack.Buildings, definition => definition.Id);
            _componentsById = BuildDictionary(pack.Components, definition => definition.Id);
            _driverArchetypesById = BuildDictionary(pack.DriverArchetypes, definition => definition.Id);
            _weatherTypesById = BuildDictionary(pack.WeatherTypes, definition => definition.Id);
        }

        public IReadOnlyList<SeriesDef> AllSeries { get; }
        public IReadOnlyList<RulesetDef> AllRulesets { get; }
        public IReadOnlyList<TrackDef> AllTracks { get; }
        public IReadOnlyList<PartTypeDef> AllPartTypes { get; }
        public IReadOnlyList<TyreCompoundDef> AllTyreCompounds { get; }
        public IReadOnlyList<SponsorDef> AllSponsors { get; }
        public IReadOnlyList<BuildingDef> AllBuildings { get; }
        public IReadOnlyList<ComponentDef> AllComponents { get; }
        public IReadOnlyList<DriverArchetypeDef> AllDriverArchetypes { get; }
        public IReadOnlyList<WeatherDef> AllWeatherTypes { get; }
        public ScalarTables Scalars { get; }

        public SeriesDef GetSeries(SeriesId id) => GetRequired(_seriesById, id, nameof(SeriesDef));
        public RulesetDef GetRuleset(RulesetId id) => GetRequired(_rulesetsById, id, nameof(RulesetDef));
        public TrackDef GetTrack(TrackId id) => GetRequired(_tracksById, id, nameof(TrackDef));
        public PartTypeDef GetPartType(PartTypeId id) => GetRequired(_partTypesById, id, nameof(PartTypeDef));
        public TyreCompoundDef GetTyreCompound(TyreCompoundId id) => GetRequired(_tyreCompoundsById, id, nameof(TyreCompoundDef));
        public SponsorDef GetSponsor(SponsorId id) => GetRequired(_sponsorsById, id, nameof(SponsorDef));
        public BuildingDef GetBuilding(BuildingId id) => GetRequired(_buildingsById, id, nameof(BuildingDef));
        public ComponentDef GetComponent(ComponentId id) => GetRequired(_componentsById, id, nameof(ComponentDef));
        public DriverArchetypeDef GetDriverArchetype(DriverArchetypeId id) => GetRequired(_driverArchetypesById, id, nameof(DriverArchetypeDef));
        public WeatherDef GetWeatherType(WeatherTypeId id) => GetRequired(_weatherTypesById, id, nameof(WeatherDef));

        private static Dictionary<TId, TDefinition> BuildDictionary<TId, TDefinition>(
            IReadOnlyList<TDefinition> definitions,
            Func<TDefinition, TId> idSelector)
        {
            var dictionary = new Dictionary<TId, TDefinition>();
            foreach (var definition in definitions)
            {
                dictionary[idSelector(definition)] = definition;
            }

            return dictionary;
        }

        private static TDefinition GetRequired<TId, TDefinition>(
            IReadOnlyDictionary<TId, TDefinition> lookup,
            TId id,
            string typeName)
        {
            if (lookup.TryGetValue(id, out var definition))
            {
                return definition;
            }

            throw new KeyNotFoundException(string.Format("Missing {0} for id '{1}'.", typeName, id));
        }
    }
}
