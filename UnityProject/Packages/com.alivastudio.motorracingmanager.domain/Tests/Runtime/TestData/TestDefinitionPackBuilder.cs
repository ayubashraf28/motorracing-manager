using System.Collections.Generic;
using MotorracingManager.Core.Definitions;
using MotorracingManager.Core.Definitions.Scalars;
using MotorracingManager.Core.Enums;
using MotorracingManager.Core.Identifiers;
using MotorracingManager.Domain.Definitions;

namespace MotorracingManager.Domain.Tests.TestData
{
    internal static class TestDefinitionPackBuilder
    {
        public static DefinitionPack CreateF1Pack()
        {
            var engineModes = new[]
            {
                new EngineModeScalar(new EngineModeId("push"), -180, 15000, 12000),
                new EngineModeScalar(new EngineModeId("standard"), 0, 10000, 10000),
                new EngineModeScalar(new EngineModeId("conserve"), 120, 7000, 8000),
            };

            var tyres = new[]
            {
                new TyreCompoundDef(new TyreCompoundId("soft"), "Soft", 9000, 300, 2, 6, 4, 400, -80, 200, 85, 110, 8),
                new TyreCompoundDef(new TyreCompoundId("medium"), "Medium", 8200, 220, 2, 10, 6, 300, -40, 150, 82, 108, 7),
                new TyreCompoundDef(new TyreCompoundId("hard"), "Hard", 7600, 150, 1, 14, 8, 220, -20, 120, 80, 106, 6),
                new TyreCompoundDef(new TyreCompoundId("inter"), "Intermediate", 7000, 200, 1, 8, 5, 350, -10, 180, 78, 100, 7),
                new TyreCompoundDef(new TyreCompoundId("wet"), "Wet", 6200, 260, 1, 6, 4, 450, -5, 220, 75, 95, 8),
            };

            var weather = new[]
            {
                new WeatherDef(new WeatherTypeId("dry"), "Dry", WeatherState.Dry, 0, 10000,
                    new Dictionary<TyreCompoundId, int> { { new TyreCompoundId("soft"), 0 }, { new TyreCompoundId("medium"), 20 }, { new TyreCompoundId("hard"), 40 }, { new TyreCompoundId("inter"), 2500 }, { new TyreCompoundId("wet"), 4200 } },
                    new Dictionary<WeatherTypeId, int> { { new WeatherTypeId("dry"), 7000 }, { new WeatherTypeId("damp"), 2000 }, { new WeatherTypeId("wet"), 900 }, { new WeatherTypeId("monsoon"), 100 } },
                    5),
                new WeatherDef(new WeatherTypeId("damp"), "Damp", WeatherState.Damp, 1200, 7000,
                    new Dictionary<TyreCompoundId, int> { { new TyreCompoundId("soft"), 900 }, { new TyreCompoundId("medium"), 700 }, { new TyreCompoundId("hard"), 600 }, { new TyreCompoundId("inter"), 0 }, { new TyreCompoundId("wet"), 600 } },
                    new Dictionary<WeatherTypeId, int> { { new WeatherTypeId("dry"), 2500 }, { new WeatherTypeId("damp"), 5000 }, { new WeatherTypeId("wet"), 2200 }, { new WeatherTypeId("monsoon"), 300 } },
                    -2),
                new WeatherDef(new WeatherTypeId("wet"), "Wet", WeatherState.Wet, 3000, 4000,
                    new Dictionary<TyreCompoundId, int> { { new TyreCompoundId("soft"), 6000 }, { new TyreCompoundId("medium"), 5500 }, { new TyreCompoundId("hard"), 5000 }, { new TyreCompoundId("inter"), 800 }, { new TyreCompoundId("wet"), 0 } },
                    new Dictionary<WeatherTypeId, int> { { new WeatherTypeId("dry"), 500 }, { new WeatherTypeId("damp"), 2000 }, { new WeatherTypeId("wet"), 6500 }, { new WeatherTypeId("monsoon"), 1000 } },
                    -8),
                new WeatherDef(new WeatherTypeId("monsoon"), "Monsoon", WeatherState.Monsoon, 5200, 2500,
                    new Dictionary<TyreCompoundId, int> { { new TyreCompoundId("soft"), 8000 }, { new TyreCompoundId("medium"), 7600 }, { new TyreCompoundId("hard"), 7200 }, { new TyreCompoundId("inter"), 2000 }, { new TyreCompoundId("wet"), 200 } },
                    new Dictionary<WeatherTypeId, int> { { new WeatherTypeId("dry"), 200 }, { new WeatherTypeId("damp"), 1200 }, { new WeatherTypeId("wet"), 4000 }, { new WeatherTypeId("monsoon"), 4600 } },
                    -11),
            };

            return new DefinitionPack(
                "test_f1",
                "1.0.0",
                new[]
                {
                    new SeriesDef(new SeriesId("f1"), "Formula 1 Championship", 1, new RulesetId("f1_2030_rules"), new[] { new TrackId("monaco"), new TrackId("silverstone") }, 10, 2),
                },
                new[]
                {
                    new RulesetDef(new RulesetId("f1_2030_rules"), "2030 F1 Regulations", new[] { 25, 18, 15, 12, 10, 8, 6, 4, 2, 1 }, 1,
                        new[] { new TyreCompoundId("soft"), new TyreCompoundId("medium"), new TyreCompoundId("hard"), new TyreCompoundId("inter"), new TyreCompoundId("wet") },
                        2, false, new[] { new EngineModeId("push"), new EngineModeId("standard"), new EngineModeId("conserve") }, 3, 14000000000, 20000, 2500,
                        QualifyingFormat.ThreeKnockout, false, new int[0], 2, true),
                },
                new[]
                {
                    new TrackDef(new TrackId("monaco"), "Monaco Grand Prix", "Monaco", 3337, 73000, 78, 19,
                        new[] { new SectorDef(0, 25000, 2000, 7), new SectorDef(1, 24000, 3000, 6), new SectorDef(2, 24000, 4000, 6) },
                        9200, 7500, 2, new Dictionary<WeatherTypeId, int> { { new WeatherTypeId("dry"), 7000 }, { new WeatherTypeId("damp"), 1500 }, { new WeatherTypeId("wet"), 1000 }, { new WeatherTypeId("monsoon"), 500 } }, 22, 38, new[] { 2 }),
                    new TrackDef(new TrackId("silverstone"), "Silverstone Grand Prix", "United Kingdom", 5891, 86000, 52, 18,
                        new[] { new SectorDef(0, 28000, 3300, 6), new SectorDef(1, 29000, 2800, 6), new SectorDef(2, 29000, 3900, 6) },
                        5600, 8400, 2, new Dictionary<WeatherTypeId, int> { { new WeatherTypeId("dry"), 5500 }, { new WeatherTypeId("damp"), 2200 }, { new WeatherTypeId("wet"), 1700 }, { new WeatherTypeId("monsoon"), 600 } }, 18, 30, new[] { 0, 2 }),
                },
                new[]
                {
                    new PartTypeDef(new PartTypeId("engine_v1"), "Standard Engine", PartSlot.Engine, 3000, 9500, 4, 500000000, 0),
                    new PartTypeDef(new PartTypeId("aero_front"), "Front Aero Package", PartSlot.Aero, 2500, 9200, 3, 300000000, 12000),
                    new PartTypeDef(new PartTypeId("chassis_v1"), "Base Chassis", PartSlot.Chassis, 3500, 9300, 5, 650000000, 45000),
                },
                tyres,
                new[]
                {
                    new SponsorDef(new SponsorId("megacorp"), "MegaCorp Industries", SponsorTier.Title, 200000000, 500000000, 7000, 5,
                        new[] { new SponsorObjectiveTemplate("Finish in top 5 in constructors", 5, "constructors_position", 100000000) }),
                },
                new[]
                {
                    new BuildingDef(new BuildingId("wind_tunnel"), "Wind Tunnel", new[]
                    {
                        new BuildingLevelDef(1, 200000000, 6, new[] { new BuildingEffectDef(BuildingEffectType.DevelopmentSpeed, 500) }),
                        new BuildingLevelDef(2, 350000000, 8, new[] { new BuildingEffectDef(BuildingEffectType.DevelopmentSpeed, 900), new BuildingEffectDef(BuildingEffectType.SetupAccuracy, 400) }),
                    }),
                },
                new[]
                {
                    new ComponentDef(new ComponentId("turbo_a"), "Turbo A", PartSlot.Engine, 500, 1500, 300, 900, 1, 25000000),
                    new ComponentDef(new ComponentId("wing_element_b"), "Wing Element B", PartSlot.Aero, 400, 1200, 200, 700, 0, 12000000),
                },
                new[]
                {
                    new DriverArchetypeDef(new DriverArchetypeId("veteran"), "Veteran", 6500, 8500, 7200, 9200, 6000, 8000, 5500, 7600, 5600, 7800, 28, 38, 5000, 8000),
                    new DriverArchetypeDef(new DriverArchetypeId("rookie_talent"), "Rookie Talent", 7000, 9200, 5000, 7800, 5800, 8200, 6200, 8800, 5000, 7600, 18, 23, 3500, 6500),
                },
                weather,
                new ScalarTables(
                    new PartRankTimeCostTable(new Dictionary<PartSlot, IReadOnlyList<int>>
                    {
                        { PartSlot.Engine, new[] { 0, 80, 160, 240, 320, 400, 480, 560, 640, 720 } },
                        { PartSlot.Aero, new[] { 0, 70, 140, 210, 280, 350, 420, 490, 560, 630 } },
                        { PartSlot.Chassis, new[] { 0, 60, 120, 180, 240, 300, 360, 420, 480, 540 } },
                    }),
                    new DriverPaceScalars(7000, 8),
                    new SetupScalars(new[] { new SetupPenaltyEntry(100, 0), new SetupPenaltyEntry(90, 70), new SetupPenaltyEntry(75, 180), new SetupPenaltyEntry(50, 350), new SetupPenaltyEntry(0, 600) }),
                    new KnowledgeScalars(300, 40),
                    new TyreTemperatureScalars(25),
                    new FuelScalars(8),
                    engineModes,
                    new DraftingScalars(300, 2000, 100),
                    new VarianceScalars(150, 5000)));
        }

        public static DefinitionPack CreateIndyCarPack()
        {
            var pack = CreateF1Pack();
            return new DefinitionPack(
                "test_indycar",
                "2.1.0",
                new[]
                {
                    new SeriesDef(new SeriesId("indycar"), "IndyCar Series", 1, new RulesetId("indy_2030_rules"), new[] { new TrackId("st_pete"), new TrackId("indy_road") }, 11, 2),
                },
                new[]
                {
                    new RulesetDef(new RulesetId("indy_2030_rules"), "2030 Indy Regulations", new[] { 50, 40, 35, 32, 30, 28, 26, 24, 22, 20 }, 0,
                        new[] { new TyreCompoundId("primary"), new TyreCompoundId("alternate"), new TyreCompoundId("wet") },
                        2, true, new[] { new EngineModeId("attack"), new EngineModeId("standard"), new EngineModeId("save") }, 2, 0, 17500, 6200,
                        QualifyingFormat.SingleSession, true, new[] { 12, 9, 7, 5, 3, 2, 1 }, 0, false),
                },
                new[]
                {
                    new TrackDef(new TrackId("st_pete"), "St. Petersburg", "United States", 2901, 70500, 100, 14,
                        new[] { new SectorDef(0, 23000, 3500, 4), new SectorDef(1, 24000, 3200, 5), new SectorDef(2, 23500, 3300, 5) },
                        5000, 8200, 3, new Dictionary<WeatherTypeId, int> { { new WeatherTypeId("dry"), 6500 }, { new WeatherTypeId("damp"), 1800 }, { new WeatherTypeId("wet"), 1300 }, { new WeatherTypeId("monsoon"), 400 } }, 24, 36, new[] { 1 }),
                    new TrackDef(new TrackId("indy_road"), "Indianapolis Road Course", "United States", 3925, 81000, 85, 14,
                        new[] { new SectorDef(0, 27000, 3000, 4), new SectorDef(1, 28000, 3600, 5), new SectorDef(2, 26000, 3400, 5) },
                        4300, 7800, 3, new Dictionary<WeatherTypeId, int> { { new WeatherTypeId("dry"), 6200 }, { new WeatherTypeId("damp"), 2000 }, { new WeatherTypeId("wet"), 1400 }, { new WeatherTypeId("monsoon"), 400 } }, 23, 34, new[] { 0, 2 }),
                },
                new[]
                {
                    new PartTypeDef(new PartTypeId("indy_engine"), "Indy Engine", PartSlot.Engine, 4200, 9000, 3, 280000000, 0),
                    new PartTypeDef(new PartTypeId("indy_aero"), "Road Aero Kit", PartSlot.Aero, 4000, 8700, 2, 180000000, 9000),
                    new PartTypeDef(new PartTypeId("indy_chassis"), "Dallara Chassis", PartSlot.Chassis, 4500, 9100, 4, 320000000, 50000),
                },
                new[]
                {
                    new TyreCompoundDef(new TyreCompoundId("primary"), "Primary", 7800, 180, 1, 15, 8, 220, -30, 100, 80, 108, 6),
                    new TyreCompoundDef(new TyreCompoundId("alternate"), "Alternate", 8600, 260, 2, 8, 5, 360, -70, 170, 84, 110, 8),
                    new TyreCompoundDef(new TyreCompoundId("wet"), "Wet", 6500, 240, 1, 7, 4, 420, -10, 210, 74, 96, 8),
                },
                pack.Sponsors,
                pack.Buildings,
                pack.Components,
                pack.DriverArchetypes,
                new[]
                {
                    new WeatherDef(new WeatherTypeId("dry"), "Dry", WeatherState.Dry, 0, 10000,
                        new Dictionary<TyreCompoundId, int> { { new TyreCompoundId("primary"), 0 }, { new TyreCompoundId("alternate"), 50 }, { new TyreCompoundId("wet"), 3800 } },
                        new Dictionary<WeatherTypeId, int> { { new WeatherTypeId("dry"), 7200 }, { new WeatherTypeId("damp"), 1900 }, { new WeatherTypeId("wet"), 800 }, { new WeatherTypeId("monsoon"), 100 } },
                        4),
                    new WeatherDef(new WeatherTypeId("damp"), "Damp", WeatherState.Damp, 1100, 7200,
                        new Dictionary<TyreCompoundId, int> { { new TyreCompoundId("primary"), 800 }, { new TyreCompoundId("alternate"), 700 }, { new TyreCompoundId("wet"), 500 } },
                        new Dictionary<WeatherTypeId, int> { { new WeatherTypeId("dry"), 2400 }, { new WeatherTypeId("damp"), 5100 }, { new WeatherTypeId("wet"), 2100 }, { new WeatherTypeId("monsoon"), 400 } },
                        -1),
                    new WeatherDef(new WeatherTypeId("wet"), "Wet", WeatherState.Wet, 2800, 4300,
                        new Dictionary<TyreCompoundId, int> { { new TyreCompoundId("primary"), 5400 }, { new TyreCompoundId("alternate"), 5900 }, { new TyreCompoundId("wet"), 0 } },
                        new Dictionary<WeatherTypeId, int> { { new WeatherTypeId("dry"), 600 }, { new WeatherTypeId("damp"), 2400 }, { new WeatherTypeId("wet"), 6000 }, { new WeatherTypeId("monsoon"), 1000 } },
                        -7),
                    new WeatherDef(new WeatherTypeId("monsoon"), "Monsoon", WeatherState.Monsoon, 4700, 2800,
                        new Dictionary<TyreCompoundId, int> { { new TyreCompoundId("primary"), 7600 }, { new TyreCompoundId("alternate"), 7800 }, { new TyreCompoundId("wet"), 300 } },
                        new Dictionary<WeatherTypeId, int> { { new WeatherTypeId("dry"), 300 }, { new WeatherTypeId("damp"), 1400 }, { new WeatherTypeId("wet"), 3900 }, { new WeatherTypeId("monsoon"), 4400 } },
                        -9),
                },
                new ScalarTables(
                    new PartRankTimeCostTable(new Dictionary<PartSlot, IReadOnlyList<int>>
                    {
                        { PartSlot.Engine, new[] { 0, 50, 100, 150, 200, 250, 300, 350, 400, 450, 500 } },
                        { PartSlot.Aero, new[] { 0, 45, 90, 135, 180, 225, 270, 315, 360, 405, 450 } },
                        { PartSlot.Chassis, new[] { 0, 40, 80, 120, 160, 200, 240, 280, 320, 360, 400 } },
                    }),
                    new DriverPaceScalars(6800, 7),
                    new SetupScalars(new[] { new SetupPenaltyEntry(100, 0), new SetupPenaltyEntry(85, 80), new SetupPenaltyEntry(70, 180), new SetupPenaltyEntry(40, 360), new SetupPenaltyEntry(0, 620) }),
                    new KnowledgeScalars(260, 50),
                    new TyreTemperatureScalars(22),
                    new FuelScalars(7),
                    new[]
                    {
                        new EngineModeScalar(new EngineModeId("attack"), -120, 13500, 11200),
                        new EngineModeScalar(new EngineModeId("standard"), 0, 10000, 10000),
                        new EngineModeScalar(new EngineModeId("save"), 140, 6800, 7600),
                    },
                    new DraftingScalars(240, 1800, 60),
                    new VarianceScalars(180, 4500)));
        }

        public static DefinitionPack Clone(
            DefinitionPack source,
            IReadOnlyList<SeriesDef> series = null,
            IReadOnlyList<RulesetDef> rulesets = null,
            IReadOnlyList<TrackDef> tracks = null,
            IReadOnlyList<PartTypeDef> partTypes = null,
            IReadOnlyList<TyreCompoundDef> tyreCompounds = null,
            IReadOnlyList<SponsorDef> sponsors = null,
            IReadOnlyList<BuildingDef> buildings = null,
            IReadOnlyList<ComponentDef> components = null,
            IReadOnlyList<DriverArchetypeDef> driverArchetypes = null,
            IReadOnlyList<WeatherDef> weatherTypes = null,
            ScalarTables scalars = null,
            string packId = null,
            string packVersion = null)
        {
            return new DefinitionPack(
                packId ?? source.PackId,
                packVersion ?? source.PackVersion,
                series ?? source.Series,
                rulesets ?? source.Rulesets,
                tracks ?? source.Tracks,
                partTypes ?? source.PartTypes,
                tyreCompounds ?? source.TyreCompounds,
                sponsors ?? source.Sponsors,
                buildings ?? source.Buildings,
                components ?? source.Components,
                driverArchetypes ?? source.DriverArchetypes,
                weatherTypes ?? source.WeatherTypes,
                scalars ?? source.Scalars);
        }
    }
}
