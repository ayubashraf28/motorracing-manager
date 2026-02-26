using MotorracingManager.Core;
using NUnit.Framework;

namespace MotorracingManager.Core.Tests
{
    public sealed class SeasonNumberTests
    {
        [Test]
        public void ToString_ReturnsWrappedValue()
        {
            var season = new SeasonNumber(2030);
            Assert.That(season.ToString(), Is.EqualTo("2030"));
        }
    }
}
