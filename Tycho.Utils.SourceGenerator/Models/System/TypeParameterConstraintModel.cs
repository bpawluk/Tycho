using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct TypeParameterConstraintModel : IEquatable<TypeParameterConstraintModel>
    {
        public TypeParameterConstraintKind Kind { get; }

        public TypeModel? Type { get; }

        public TypeParameterConstraintModel(TypeParameterConstraintKind kind, TypeModel? type)
        {
            Kind = kind;
            Type = type;
        }

        public bool Equals(TypeParameterConstraintModel other)
        {
            bool bothTypesNull = Type is null && other.Type is null;
            return Kind == other.Kind && (bothTypesNull || Type?.Equals(other.Type) == true);
        }

        public override bool Equals(object obj)
        {
            return obj is TypeParameterConstraintModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Kind.GetHashCode(), Type?.GetHashCode() ?? 0);
        }

        public static bool operator ==(TypeParameterConstraintModel left, TypeParameterConstraintModel right) => left.Equals(right);

        public static bool operator !=(TypeParameterConstraintModel left, TypeParameterConstraintModel right) => !left.Equals(right);
    }
}
