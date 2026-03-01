using System.Collections.Generic;
using MotorracingManager.Core.Identifiers;
using MotorracingManager.Domain.Definitions;
using MotorracingManager.Domain.Tests.TestData;
using NUnit.Framework;

namespace MotorracingManager.Domain.Tests.Definitions
{
    public sealed class DefinitionRegistryTests
    {
        [Test]
        public void GetMethods_ReturnDefinitions_ForKnownIds()
        {
            var pack = TestDefinitionPackBuilder.CreateF1Pack();
            var registry = new DefinitionRegistry(pack);

            Assert.That(registry.GetSeries(new SeriesId("f1")).DisplayName, Is.EqualTo("Formula 1 Championship"));
            Assert.That(registry.GetRuleset(new RulesetId("f1_2030_rules")).DisplayName, Is.EqualTo("2030 F1 Regulations"));
            Assert.That(registry.GetTrack(new TrackId("monaco")).DisplayName, Is.EqualTo("Monaco Grand Prix"));
            Assert.That(registry.GetPartType(new PartTypeId("engine_v1")).DisplayName, Is.EqualTo("Standard Engine"));
            Assert.That(registry.GetTyreCompound(new TyreCompoundId("soft")).DisplayName, Is.EqualTo("Soft"));
            Assert.That(registry.GetSponsor(new SponsorId("megacorp")).DisplayName, Is.EqualTo("MegaCorp Industries"));
            Assert.That(registry.GetBuilding(new BuildingId("wind_tunnel")).DisplayName, Is.EqualTo("Wind Tunnel"));
            Assert.That(registry.GetComponent(new ComponentId("turbo_a")).DisplayName, Is.EqualTo("Turbo A"));
            Assert.That(registry.GetDriverArchetype(new DriverArchetypeId("veteran")).DisplayName, Is.EqualTo("Veteran"));
            Assert.That(registry.GetWeatherType(new WeatherTypeId("dry")).DisplayName, Is.EqualTo("Dry"));
        }

        [Test]
        public void GetMethods_Throw_ForMissingIds()
        {
            var registry = new DefinitionRegistry(TestDefinitionPackBuilder.CreateF1Pack());

            Assert.Throws<KeyNotFoundException>(() => registry.GetTrack(new TrackId("missing_track")));
        }

        [Test]
        public void AllCollections_MatchPackCounts_AndExposeScalars()
        {
            var pack = TestDefinitionPackBuilder.CreateF1Pack();
            var registry = new DefinitionRegistry(pack);

            Assert.That(registry.AllSeries.Count, Is.EqualTo(pack.Series.Count));
            Assert.That(registry.AllRulesets.Count, Is.EqualTo(pack.Rulesets.Count));
            Assert.That(registry.AllTracks.Count, Is.EqualTo(pack.Tracks.Count));
            Assert.That(registry.AllPartTypes.Count, Is.EqualTo(pack.PartTypes.Count));
            Assert.That(registry.AllTyreCompounds.Count, Is.EqualTo(pack.TyreCompounds.Count));
            Assert.That(registry.AllSponsors.Count, Is.EqualTo(pack.Sponsors.Count));
            Assert.That(registry.AllBuildings.Count, Is.EqualTo(pack.Buildings.Count));
            Assert.That(registry.AllComponents.Count, Is.EqualTo(pack.Components.Count));
            Assert.That(registry.AllDriverArchetypes.Count, Is.EqualTo(pack.DriverArchetypes.Count));
            Assert.That(registry.AllWeatherTypes.Count, Is.EqualTo(pack.WeatherTypes.Count));
            Assert.That(registry.Scalars, Is.SameAs(pack.Scalars));
        }

        [Test]
        public void RegistriesRemainIsolatedAcrossPackSwaps()
        {
            var validator = new DefinitionPackValidator();
            var f1Pack = TestDefinitionPackBuilder.CreateF1Pack();
            var indyPack = TestDefinitionPackBuilder.CreateIndyCarPack();
            var f1Registry = new DefinitionRegistry(f1Pack);
            var indyRegistry = new DefinitionRegistry(indyPack);

            Assert.That(validator.Validate(f1Pack).IsValid, Is.True);
            Assert.That(validator.Validate(indyPack).IsValid, Is.True);
            Assert.That(f1Registry.GetSeries(new SeriesId("f1")).DisplayName, Is.EqualTo("Formula 1 Championship"));
            Assert.That(indyRegistry.GetSeries(new SeriesId("indycar")).DisplayName, Is.EqualTo("IndyCar Series"));
            Assert.That(f1Registry.AllTracks[0].Id, Is.Not.EqualTo(indyRegistry.AllTracks[0].Id));
            Assert.That(f1Registry.GetTyreCompound(new TyreCompoundId("soft")).DisplayName, Is.EqualTo("Soft"));
            Assert.That(indyRegistry.GetTyreCompound(new TyreCompoundId("primary")).DisplayName, Is.EqualTo("Primary"));
        }
    }
}
