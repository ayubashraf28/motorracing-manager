using System;
using System.Collections.Generic;

namespace MotorracingManager.Core.Identifiers
{
    internal static class IdentifierUtility
    {
        public static string RequireValue(string value, string paramName)
        {
            return Guard.AgainstNullOrWhiteSpace(value, paramName);
        }

        public static bool Equals(string left, string right)
        {
            return StringComparer.Ordinal.Equals(left ?? string.Empty, right ?? string.Empty);
        }

        public static int GetHashCode(string value)
        {
            return StringComparer.Ordinal.GetHashCode(value ?? string.Empty);
        }
    }

    public readonly struct SeriesId : IEquatable<SeriesId>
    {
        public SeriesId(string value)
        {
            Value = IdentifierUtility.RequireValue(value, nameof(value));
        }

        public string Value { get; }

        public bool Equals(SeriesId other) => IdentifierUtility.Equals(Value, other.Value);
        public override bool Equals(object obj) => obj is SeriesId other && Equals(other);
        public override int GetHashCode() => IdentifierUtility.GetHashCode(Value);
        public override string ToString() => Value ?? string.Empty;
        public static bool operator ==(SeriesId left, SeriesId right) => left.Equals(right);
        public static bool operator !=(SeriesId left, SeriesId right) => !left.Equals(right);
    }

    public readonly struct TrackId : IEquatable<TrackId>
    {
        public TrackId(string value)
        {
            Value = IdentifierUtility.RequireValue(value, nameof(value));
        }

        public string Value { get; }

        public bool Equals(TrackId other) => IdentifierUtility.Equals(Value, other.Value);
        public override bool Equals(object obj) => obj is TrackId other && Equals(other);
        public override int GetHashCode() => IdentifierUtility.GetHashCode(Value);
        public override string ToString() => Value ?? string.Empty;
        public static bool operator ==(TrackId left, TrackId right) => left.Equals(right);
        public static bool operator !=(TrackId left, TrackId right) => !left.Equals(right);
    }

    public readonly struct RulesetId : IEquatable<RulesetId>
    {
        public RulesetId(string value)
        {
            Value = IdentifierUtility.RequireValue(value, nameof(value));
        }

        public string Value { get; }

        public bool Equals(RulesetId other) => IdentifierUtility.Equals(Value, other.Value);
        public override bool Equals(object obj) => obj is RulesetId other && Equals(other);
        public override int GetHashCode() => IdentifierUtility.GetHashCode(Value);
        public override string ToString() => Value ?? string.Empty;
        public static bool operator ==(RulesetId left, RulesetId right) => left.Equals(right);
        public static bool operator !=(RulesetId left, RulesetId right) => !left.Equals(right);
    }

    public readonly struct PartTypeId : IEquatable<PartTypeId>
    {
        public PartTypeId(string value)
        {
            Value = IdentifierUtility.RequireValue(value, nameof(value));
        }

        public string Value { get; }

        public bool Equals(PartTypeId other) => IdentifierUtility.Equals(Value, other.Value);
        public override bool Equals(object obj) => obj is PartTypeId other && Equals(other);
        public override int GetHashCode() => IdentifierUtility.GetHashCode(Value);
        public override string ToString() => Value ?? string.Empty;
        public static bool operator ==(PartTypeId left, PartTypeId right) => left.Equals(right);
        public static bool operator !=(PartTypeId left, PartTypeId right) => !left.Equals(right);
    }

    public readonly struct TyreCompoundId : IEquatable<TyreCompoundId>
    {
        public TyreCompoundId(string value)
        {
            Value = IdentifierUtility.RequireValue(value, nameof(value));
        }

        public string Value { get; }

        public bool Equals(TyreCompoundId other) => IdentifierUtility.Equals(Value, other.Value);
        public override bool Equals(object obj) => obj is TyreCompoundId other && Equals(other);
        public override int GetHashCode() => IdentifierUtility.GetHashCode(Value);
        public override string ToString() => Value ?? string.Empty;
        public static bool operator ==(TyreCompoundId left, TyreCompoundId right) => left.Equals(right);
        public static bool operator !=(TyreCompoundId left, TyreCompoundId right) => !left.Equals(right);
    }

    public readonly struct SponsorId : IEquatable<SponsorId>
    {
        public SponsorId(string value)
        {
            Value = IdentifierUtility.RequireValue(value, nameof(value));
        }

        public string Value { get; }

        public bool Equals(SponsorId other) => IdentifierUtility.Equals(Value, other.Value);
        public override bool Equals(object obj) => obj is SponsorId other && Equals(other);
        public override int GetHashCode() => IdentifierUtility.GetHashCode(Value);
        public override string ToString() => Value ?? string.Empty;
        public static bool operator ==(SponsorId left, SponsorId right) => left.Equals(right);
        public static bool operator !=(SponsorId left, SponsorId right) => !left.Equals(right);
    }

    public readonly struct BuildingId : IEquatable<BuildingId>
    {
        public BuildingId(string value)
        {
            Value = IdentifierUtility.RequireValue(value, nameof(value));
        }

        public string Value { get; }

        public bool Equals(BuildingId other) => IdentifierUtility.Equals(Value, other.Value);
        public override bool Equals(object obj) => obj is BuildingId other && Equals(other);
        public override int GetHashCode() => IdentifierUtility.GetHashCode(Value);
        public override string ToString() => Value ?? string.Empty;
        public static bool operator ==(BuildingId left, BuildingId right) => left.Equals(right);
        public static bool operator !=(BuildingId left, BuildingId right) => !left.Equals(right);
    }

    public readonly struct ComponentId : IEquatable<ComponentId>
    {
        public ComponentId(string value)
        {
            Value = IdentifierUtility.RequireValue(value, nameof(value));
        }

        public string Value { get; }

        public bool Equals(ComponentId other) => IdentifierUtility.Equals(Value, other.Value);
        public override bool Equals(object obj) => obj is ComponentId other && Equals(other);
        public override int GetHashCode() => IdentifierUtility.GetHashCode(Value);
        public override string ToString() => Value ?? string.Empty;
        public static bool operator ==(ComponentId left, ComponentId right) => left.Equals(right);
        public static bool operator !=(ComponentId left, ComponentId right) => !left.Equals(right);
    }

    public readonly struct DriverArchetypeId : IEquatable<DriverArchetypeId>
    {
        public DriverArchetypeId(string value)
        {
            Value = IdentifierUtility.RequireValue(value, nameof(value));
        }

        public string Value { get; }

        public bool Equals(DriverArchetypeId other) => IdentifierUtility.Equals(Value, other.Value);
        public override bool Equals(object obj) => obj is DriverArchetypeId other && Equals(other);
        public override int GetHashCode() => IdentifierUtility.GetHashCode(Value);
        public override string ToString() => Value ?? string.Empty;
        public static bool operator ==(DriverArchetypeId left, DriverArchetypeId right) => left.Equals(right);
        public static bool operator !=(DriverArchetypeId left, DriverArchetypeId right) => !left.Equals(right);
    }

    public readonly struct WeatherTypeId : IEquatable<WeatherTypeId>
    {
        public WeatherTypeId(string value)
        {
            Value = IdentifierUtility.RequireValue(value, nameof(value));
        }

        public string Value { get; }

        public bool Equals(WeatherTypeId other) => IdentifierUtility.Equals(Value, other.Value);
        public override bool Equals(object obj) => obj is WeatherTypeId other && Equals(other);
        public override int GetHashCode() => IdentifierUtility.GetHashCode(Value);
        public override string ToString() => Value ?? string.Empty;
        public static bool operator ==(WeatherTypeId left, WeatherTypeId right) => left.Equals(right);
        public static bool operator !=(WeatherTypeId left, WeatherTypeId right) => !left.Equals(right);
    }

    public readonly struct EngineModeId : IEquatable<EngineModeId>
    {
        public EngineModeId(string value)
        {
            Value = IdentifierUtility.RequireValue(value, nameof(value));
        }

        public string Value { get; }

        public bool Equals(EngineModeId other) => IdentifierUtility.Equals(Value, other.Value);
        public override bool Equals(object obj) => obj is EngineModeId other && Equals(other);
        public override int GetHashCode() => IdentifierUtility.GetHashCode(Value);
        public override string ToString() => Value ?? string.Empty;
        public static bool operator ==(EngineModeId left, EngineModeId right) => left.Equals(right);
        public static bool operator !=(EngineModeId left, EngineModeId right) => !left.Equals(right);
    }
}
