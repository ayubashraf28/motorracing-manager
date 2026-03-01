using System;
using MotorracingManager.Core.Identifiers;
using NUnit.Framework;

namespace MotorracingManager.Core.Tests.Identifiers
{
    public sealed class DefinitionIdentifierTests
    {
        [Test] public void SeriesId_ImplementsContracts() => AssertIdentifierContracts(value => new SeriesId(value), (left, right) => left == right, (left, right) => left != right);
        [Test] public void TrackId_ImplementsContracts() => AssertIdentifierContracts(value => new TrackId(value), (left, right) => left == right, (left, right) => left != right);
        [Test] public void RulesetId_ImplementsContracts() => AssertIdentifierContracts(value => new RulesetId(value), (left, right) => left == right, (left, right) => left != right);
        [Test] public void PartTypeId_ImplementsContracts() => AssertIdentifierContracts(value => new PartTypeId(value), (left, right) => left == right, (left, right) => left != right);
        [Test] public void TyreCompoundId_ImplementsContracts() => AssertIdentifierContracts(value => new TyreCompoundId(value), (left, right) => left == right, (left, right) => left != right);
        [Test] public void SponsorId_ImplementsContracts() => AssertIdentifierContracts(value => new SponsorId(value), (left, right) => left == right, (left, right) => left != right);
        [Test] public void BuildingId_ImplementsContracts() => AssertIdentifierContracts(value => new BuildingId(value), (left, right) => left == right, (left, right) => left != right);
        [Test] public void ComponentId_ImplementsContracts() => AssertIdentifierContracts(value => new ComponentId(value), (left, right) => left == right, (left, right) => left != right);
        [Test] public void DriverArchetypeId_ImplementsContracts() => AssertIdentifierContracts(value => new DriverArchetypeId(value), (left, right) => left == right, (left, right) => left != right);
        [Test] public void WeatherTypeId_ImplementsContracts() => AssertIdentifierContracts(value => new WeatherTypeId(value), (left, right) => left == right, (left, right) => left != right);
        [Test] public void EngineModeId_ImplementsContracts() => AssertIdentifierContracts(value => new EngineModeId(value), (left, right) => left == right, (left, right) => left != right);

        private static void AssertIdentifierContracts<T>(
            Func<string, T> factory,
            Func<T, T, bool> equalsOperator,
            Func<T, T, bool> notEqualsOperator)
        {
            var expectedValue = "alpha";
            var equalLeft = factory(expectedValue);
            var equalRight = factory(expectedValue);
            var different = factory("beta");

            Assert.That(equalLeft.ToString(), Is.EqualTo(expectedValue));
            Assert.That(equalLeft.Equals(equalRight), Is.True);
            Assert.That(equalLeft.Equals(different), Is.False);
            Assert.That(equalLeft.GetHashCode(), Is.EqualTo(equalRight.GetHashCode()));
            Assert.That(equalsOperator(equalLeft, equalRight), Is.True);
            Assert.That(notEqualsOperator(equalLeft, equalRight), Is.False);
            Assert.That(equalsOperator(equalLeft, different), Is.False);
            Assert.That(notEqualsOperator(equalLeft, different), Is.True);
            Assert.That(((object)equalLeft).Equals(equalRight), Is.True);

            Assert.Throws<ArgumentException>(() => factory(null));
            Assert.Throws<ArgumentException>(() => factory(string.Empty));
            Assert.Throws<ArgumentException>(() => factory("   "));
        }
    }
}
