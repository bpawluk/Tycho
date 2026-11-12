using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct TypeArgumentModel : IEquatable<TypeArgumentModel>
    {
        public string Name { get; }

        public TypeReferenceModel Value { get; }

        public TypeArgumentModel(string name, TypeReferenceModel value)
        {
            Name = name ?? string.Empty;
            Value = value;
        }

        public bool Matches(TypeArgumentModel other)
        {
            return string.Equals(Name, other.Name, StringComparison.Ordinal)
                && Value.Matches(other.Value);
        }

        public bool Equals(TypeArgumentModel other)
        {
            return string.Equals(Name, other.Name, StringComparison.Ordinal)
                && Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is TypeArgumentModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                StringComparer.Ordinal.GetHashCode(Name),
                Value.GetHashCode());
        }

        public static bool operator ==(TypeArgumentModel left, TypeArgumentModel right) => left.Equals(right);

        public static bool operator !=(TypeArgumentModel left, TypeArgumentModel right) => !left.Equals(right);
    }
}
