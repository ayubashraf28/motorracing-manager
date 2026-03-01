using System.Collections.Generic;
using MotorracingManager.Core.Definitions.Scalars;
using MotorracingManager.Core.Enums;
using NUnit.Framework;

namespace MotorracingManager.Core.Tests.Definitions.Scalars
{
    public sealed class PartRankTimeCostTableTests
    {
        [Test]
        public void LookupMs_ReturnsExactValue_ForRankWithinRange()
        {
            var table = CreateTable();

            Assert.That(table.LookupMs(PartSlot.Engine, 2), Is.EqualTo(80));
        }

        [Test]
        public void LookupMs_ClampsToFirstEntry_ForZeroOrNegativeRank()
        {
            var table = CreateTable();

            Assert.That(table.LookupMs(PartSlot.Engine, 0), Is.EqualTo(0));
            Assert.That(table.LookupMs(PartSlot.Engine, -3), Is.EqualTo(0));
        }

        [Test]
        public void LookupMs_ClampsToLastEntry_ForOversizedRank()
        {
            var table = CreateTable();

            Assert.That(table.LookupMs(PartSlot.Engine, 99), Is.EqualTo(160));
        }

        private static PartRankTimeCostTable CreateTable()
        {
            return new PartRankTimeCostTable(new Dictionary<PartSlot, IReadOnlyList<int>>
            {
                { PartSlot.Engine, new[] { 0, 80, 160 } },
                { PartSlot.Aero, new[] { 0, 70, 140 } },
                { PartSlot.Chassis, new[] { 0, 60, 120 } },
            });
        }
    }
}
