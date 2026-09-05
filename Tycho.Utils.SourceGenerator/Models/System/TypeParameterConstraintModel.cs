using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct TypeParameterConstraintModel : IEquatable<TypeParameterConstraintModel>
    {
        public static TypeParameterConstraintModel ReferenceType { get; } = new TypeParameterConstraintModel("class", type: null);

        public static TypeParameterConstraintModel NullableReferenceType { get; } = new TypeParameterConstraintModel("class?", type: null);

        public static TypeParameterConstraintModel ValueType { get; } = new TypeParameterConstraintModel("struct", type: null);

        public static TypeParameterConstraintModel Unmanaged { get; } = new TypeParameterConstraintModel("unmanaged", type: null);

        public static TypeParameterConstraintModel NotNull { get; } = new TypeParameterConstraintModel("notnull", type: null);

        public static TypeParameterConstraintModel Constructor { get; } = new TypeParameterConstraintModel("new()", type: null);

        public static TypeParameterConstraintModel AllowsRefStruct { get; } = new TypeParameterConstraintModel("allows ref struct", type: null);

        public static TypeParameterConstraintModel TypeConstraint(TypeReferenceModel type) => new TypeParameterConstraintModel(type.FullReferenceName, type);

        public string Keyword { get; }

        public TypeReferenceModel? Type { get; }

        private TypeParameterConstraintModel(string keyword, TypeReferenceModel? type)
        {
            Keyword = keyword ?? string.Empty;
            Type = type;
        }

        public bool Equals(TypeParameterConstraintModel other)
        {
            bool bothTypesNull = Type is null && other.Type is null;
            return string.Equals(Keyword, other.Keyword, StringComparison.Ordinal)
                && (bothTypesNull || Type?.Equals(other.Type) == true);
        }

        public override bool Equals(object obj)
        {
            return obj is TypeParameterConstraintModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                StringComparer.Ordinal.GetHashCode(Keyword),
                Type?.GetHashCode() ?? 0);
        }

        public override string ToString() => Keyword;

        public static bool operator ==(TypeParameterConstraintModel left, TypeParameterConstraintModel right) => left.Equals(right);

        public static bool operator !=(TypeParameterConstraintModel left, TypeParameterConstraintModel right) => !left.Equals(right);
    }
}
