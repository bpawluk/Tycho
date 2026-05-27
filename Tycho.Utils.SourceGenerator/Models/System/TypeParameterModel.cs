using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct TypeParameterModel : IEquatable<TypeParameterModel>
    {
        public string Name { get; }

        public ImmutableEquatableArray<TypeParameterConstraintModel> Constraints { get; }

        public TypeParameterModel(string name, ImmutableEquatableArray<TypeParameterConstraintModel> constraints)
        {
            Name = name;
            Constraints = constraints ?? ImmutableEquatableArray<TypeParameterConstraintModel>.Empty;
        }

        public bool Equals(TypeParameterModel other)
        {
            return string.Equals(Name, other.Name, StringComparison.Ordinal)
                && Constraints.Equals(other.Constraints);
        }

        public override bool Equals(object obj)
        {
            return obj is TypeParameterModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                StringComparer.Ordinal.GetHashCode(Name ?? string.Empty),
                Constraints.GetHashCode());
        }

        public static bool operator ==(TypeParameterModel left, TypeParameterModel right) => left.Equals(right);

        public static bool operator !=(TypeParameterModel left, TypeParameterModel right) => !left.Equals(right);
    }
}
