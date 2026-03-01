using System.Collections.Generic;
using MotorracingManager.Core.Enums;
using MotorracingManager.Core.Identifiers;

namespace MotorracingManager.Domain.Definitions
{
    public sealed class SeriesDef
    {
        public SeriesDef(
            SeriesId id,
            string displayName,
            int tier,
            RulesetId rulesetId,
            IReadOnlyList<TrackId> calendarTemplate,
            int teamCount,
            int driversPerTeam)
        {
            Guard.AgainstDefaultIdentifier(id.Value, nameof(id));
            Guard.AgainstDefaultIdentifier(rulesetId.Value, nameof(rulesetId));
            Id = id;
            DisplayName = Guard.AgainstNullOrWhiteSpace(displayName, nameof(displayName));
            Tier = tier;
            RulesetId = rulesetId;
            CalendarTemplate = Guard.AgainstNullOrCopy(calendarTemplate, nameof(calendarTemplate));
            TeamCount = teamCount;
            DriversPerTeam = driversPerTeam;
        }

        public SeriesId Id { get; }
        public string DisplayName { get; }
        public int Tier { get; }
        public RulesetId RulesetId { get; }
        public IReadOnlyList<TrackId> CalendarTemplate { get; }
        public int TeamCount { get; }
        public int DriversPerTeam { get; }
    }

    public sealed class RulesetDef
    {
        public RulesetDef(
            RulesetId id,
            string displayName,
            IReadOnlyList<int> pointsTable,
            int fastestLapBonusPoints,
            IReadOnlyList<TyreCompoundId> availableCompounds,
            int minCompoundsPerRace,
            bool refuellingAllowed,
            IReadOnlyList<EngineModeId> availableEngineModes,
            int partLimitPerCategory,
            long budgetCapCents,
            int pitLaneTimeLossMs,
            int pitStopBaseTimeMs,
            QualifyingFormat qualifyingFormat,
            bool sprintRaceEnabled,
            IReadOnlyList<int> sprintPointsTable,
            int drsDisabledLaps,
            bool drsDisabledInWet)
        {
            Guard.AgainstDefaultIdentifier(id.Value, nameof(id));
            Id = id;
            DisplayName = Guard.AgainstNullOrWhiteSpace(displayName, nameof(displayName));
            PointsTable = Guard.AgainstNullOrCopy(pointsTable, nameof(pointsTable));
            FastestLapBonusPoints = fastestLapBonusPoints;
            AvailableCompounds = Guard.AgainstNullOrCopy(availableCompounds, nameof(availableCompounds));
            MinCompoundsPerRace = minCompoundsPerRace;
            RefuellingAllowed = refuellingAllowed;
            AvailableEngineModes = Guard.AgainstNullOrCopy(availableEngineModes, nameof(availableEngineModes));
            PartLimitPerCategory = partLimitPerCategory;
            BudgetCapCents = budgetCapCents;
            PitLaneTimeLossMs = pitLaneTimeLossMs;
            PitStopBaseTimeMs = pitStopBaseTimeMs;
            QualifyingFormat = qualifyingFormat;
            SprintRaceEnabled = sprintRaceEnabled;
            SprintPointsTable = Guard.AgainstNullOrCopy(sprintPointsTable, nameof(sprintPointsTable));
            DrsDisabledLaps = drsDisabledLaps;
            DrsDisabledInWet = drsDisabledInWet;
        }

        public RulesetId Id { get; }
        public string DisplayName { get; }
        public IReadOnlyList<int> PointsTable { get; }
        public int FastestLapBonusPoints { get; }
        public IReadOnlyList<TyreCompoundId> AvailableCompounds { get; }
        public int MinCompoundsPerRace { get; }
        public bool RefuellingAllowed { get; }
        public IReadOnlyList<EngineModeId> AvailableEngineModes { get; }
        public int PartLimitPerCategory { get; }
        public long BudgetCapCents { get; }
        public int PitLaneTimeLossMs { get; }
        public int PitStopBaseTimeMs { get; }
        public QualifyingFormat QualifyingFormat { get; }
        public bool SprintRaceEnabled { get; }
        public IReadOnlyList<int> SprintPointsTable { get; }
        public int DrsDisabledLaps { get; }
        public bool DrsDisabledInWet { get; }
    }
}
