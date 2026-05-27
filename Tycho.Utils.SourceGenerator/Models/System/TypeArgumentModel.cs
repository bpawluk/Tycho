using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct TypeArgumentModel : IEquatable<TypeArgumentModel>
    {
        public string Name { get; }

        public TypeModel Value { get; }

        public TypeArgumentModel(string name, TypeModel value)
        {
            Name = name;
            Value = value;
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
                StringComparer.Ordinal.GetHashCode(Name ?? string.Empty),
                Value.GetHashCode());
        }

        public static bool operator ==(TypeArgumentModel left, TypeArgumentModel right) => left.Equals(right);

        public static bool operator !=(TypeArgumentModel left, TypeArgumentModel right) => !left.Equals(right);
    }
}
