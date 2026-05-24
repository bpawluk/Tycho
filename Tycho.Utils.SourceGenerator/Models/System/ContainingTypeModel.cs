using System;
using System.Linq;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct ContainingTypeModel : IEquatable<ContainingTypeModel>
    {
        public string Name { get; }

        public ImmutableEquatableArray<string> TypeParameters { get; }

        public ImmutableEquatableArray<string> TypeParameterConstraintClauses { get; }

        public ImmutableEquatableArray<string> TypeArguments { get; }

        public string TypeParametersSuffix => BuildTypeSuffix(TypeParameters);

        public string TypeArgumentsSuffix => BuildTypeSuffix(TypeArguments.Count == 0 ? TypeParameters : TypeArguments);

        public string DeclarationName => $"{Name}{TypeParametersSuffix}";

        public string ReferenceName => $"{Name}{TypeArgumentsSuffix}";

        public string MetadataName => TypeParameters.Count > 0 ? $"{Name}`{TypeParameters.Count}" : Name;

        public string DeclarationSignature
        {
            get
            {
                ImmutableEquatableArray<string> constraints = TypeParameterConstraintClauses;
                if (constraints.Count == 0)
                {
                    return DeclarationName;
                }

                return $"{DeclarationName} {string.Join(" ", constraints.Where(clause => !string.IsNullOrWhiteSpace(clause)))}";
            }
        }

        public ContainingTypeModel(
            string typeName,
            ImmutableEquatableArray<string> typeParameters,
            ImmutableEquatableArray<string> typeParameterConstraintClauses,
            ImmutableEquatableArray<string> typeArguments)
        {
            Name = typeName;
            TypeParameters = typeParameters ?? ImmutableEquatableArray<string>.Empty;
            TypeParameterConstraintClauses = typeParameterConstraintClauses ?? ImmutableEquatableArray<string>.Empty;
            TypeArguments = typeArguments ?? ImmutableEquatableArray<string>.Empty;
        }

        public bool Equals(ContainingTypeModel other)
        {
            return string.Equals(Name, other.Name, StringComparison.Ordinal)
                && TypeParameters.Equals(other.TypeParameters)
                && TypeParameterConstraintClauses.Equals(other.TypeParameterConstraintClauses)
                && TypeArguments.Equals(other.TypeArguments);
        }

        public override bool Equals(object obj)
        {
            return obj is ContainingTypeModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                StringComparer.Ordinal.GetHashCode(Name ?? string.Empty),
                TypeParameters.GetHashCode(),
                TypeParameterConstraintClauses.GetHashCode(),
                TypeArguments.GetHashCode());
        }

        public static bool operator ==(ContainingTypeModel left, ContainingTypeModel right) => left.Equals(right);

        public static bool operator !=(ContainingTypeModel left, ContainingTypeModel right) => !left.Equals(right);

        private static string BuildTypeSuffix(ImmutableEquatableArray<string> values)
        {
            return values.Count == 0 ? string.Empty : $"<{string.Join(", ", values)}>";
        }
    }
}
