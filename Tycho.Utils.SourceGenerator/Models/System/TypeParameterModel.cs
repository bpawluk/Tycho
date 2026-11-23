using System;
using System.Linq;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct TypeParameterModel : IEquatable<TypeParameterModel>
    {
        public string Name { get; }

        public ImmutableEquatableArray<TypeParameterConstraintModel> Constraints { get; }

        public string ConstraintsClause => Constraints.Count == 0
                ? string.Empty
                : $"where {Name} : {string.Join(", ", Constraints.Select(constraint => constraint.ToString()))}";

        public TypeParameterModel(string name, ImmutableEquatableArray<TypeParameterConstraintModel> constraints)
        {
            Name = name ?? string.Empty;
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
                StringComparer.Ordinal.GetHashCode(Name),
                Constraints.GetHashCode());
        }

        public static bool operator ==(TypeParameterModel left, TypeParameterModel right) => left.Equals(right);

        public static bool operator !=(TypeParameterModel left, TypeParameterModel right) => !left.Equals(right);
    }
}
