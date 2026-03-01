using System;
using System.Collections.Generic;
using MotorracingManager.Core.Definitions;
using MotorracingManager.Core.Definitions.Scalars;

namespace MotorracingManager.Domain.Definitions
{
    public sealed class DefinitionPack
    {
        public DefinitionPack(
            string packId,
            string packVersion,
            IReadOnlyList<SeriesDef> series,
            IReadOnlyList<RulesetDef> rulesets,
            IReadOnlyList<TrackDef> tracks,
            IReadOnlyList<PartTypeDef> partTypes,
            IReadOnlyList<TyreCompoundDef> tyreCompounds,
            IReadOnlyList<SponsorDef> sponsors,
            IReadOnlyList<BuildingDef> buildings,
            IReadOnlyList<ComponentDef> components,
            IReadOnlyList<DriverArchetypeDef> driverArchetypes,
            IReadOnlyList<WeatherDef> weatherTypes,
            ScalarTables scalars)
        {
            PackId = Guard.AgainstNullOrWhiteSpace(packId, nameof(packId));
            PackVersion = Guard.AgainstNullOrWhiteSpace(packVersion, nameof(packVersion));
            Series = Guard.AgainstNullOrCopy(series, nameof(series));
            Rulesets = Guard.AgainstNullOrCopy(rulesets, nameof(rulesets));
            Tracks = Guard.AgainstNullOrCopy(tracks, nameof(tracks));
            PartTypes = Guard.AgainstNullOrCopy(partTypes, nameof(partTypes));
            TyreCompounds = Guard.AgainstNullOrCopy(tyreCompounds, nameof(tyreCompounds));
            Sponsors = Guard.AgainstNullOrCopy(sponsors, nameof(sponsors));
            Buildings = Guard.AgainstNullOrCopy(buildings, nameof(buildings));
            Components = Guard.AgainstNullOrCopy(components, nameof(components));
            DriverArchetypes = Guard.AgainstNullOrCopy(driverArchetypes, nameof(driverArchetypes));
            WeatherTypes = Guard.AgainstNullOrCopy(weatherTypes, nameof(weatherTypes));
            Scalars = scalars ?? throw new ArgumentNullException(nameof(scalars));
        }

        public string PackId { get; }
        public string PackVersion { get; }
        public IReadOnlyList<SeriesDef> Series { get; }
        public IReadOnlyList<RulesetDef> Rulesets { get; }
        public IReadOnlyList<TrackDef> Tracks { get; }
        public IReadOnlyList<PartTypeDef> PartTypes { get; }
        public IReadOnlyList<TyreCompoundDef> TyreCompounds { get; }
        public IReadOnlyList<SponsorDef> Sponsors { get; }
        public IReadOnlyList<BuildingDef> Buildings { get; }
        public IReadOnlyList<ComponentDef> Components { get; }
        public IReadOnlyList<DriverArchetypeDef> DriverArchetypes { get; }
        public IReadOnlyList<WeatherDef> WeatherTypes { get; }
        public ScalarTables Scalars { get; }
    }
}
