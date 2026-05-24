using System;
using System.Linq;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct ContainingTypeModel : IEquatable<ContainingTypeModel>
    {
        public TypeKind Kind { get; }

        public ImmutableEquatableArray<string> Modifiers { get; }

        public string Name { get; }

        public ImmutableEquatableArray<string> TypeParameters { get; }

        public ImmutableEquatableArray<string> TypeParameterConstraintClauses { get; }

        public ImmutableEquatableArray<string> TypeArguments { get; }

        public string TypeParametersSuffix => BuildTypeSuffix(TypeParameters);

        public string TypeArgumentsSuffix => BuildTypeSuffix(TypeArguments.Count == 0 ? TypeParameters : TypeArguments);

        public string DeclarationName => $"{Name}{TypeParametersSuffix}";

        public string ReferenceName => $"{Name}{TypeArgumentsSuffix}";

        public string MetadataName => TypeParameters.Count > 0 ? $"{Name}`{TypeParameters.Count}" : Name;

        public string DeclarationSignature => BuildTypeDeclaration();

        public ContainingTypeModel(
            TypeKind kind,
            ImmutableEquatableArray<string> modifiers,
            string typeName,
            ImmutableEquatableArray<string> typeParameters,
            ImmutableEquatableArray<string> typeParameterConstraintClauses,
            ImmutableEquatableArray<string> typeArguments)
        {
            Kind = kind;
            Modifiers = modifiers ?? ImmutableEquatableArray<string>.Empty;
            Name = typeName;
            TypeParameters = typeParameters ?? ImmutableEquatableArray<string>.Empty;
            TypeParameterConstraintClauses = typeParameterConstraintClauses ?? ImmutableEquatableArray<string>.Empty;
            TypeArguments = typeArguments ?? ImmutableEquatableArray<string>.Empty;
        }

        public bool Equals(ContainingTypeModel other)
        {
            return Kind == other.Kind
                && Modifiers.Equals(other.Modifiers)
                && string.Equals(Name, other.Name, StringComparison.Ordinal)
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
                Kind.GetHashCode(),
                Modifiers.GetHashCode(),
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

        private string BuildTypeDeclaration()
        {
            string kindKeyword = Kind switch
            {
                TypeKind.Interface => "interface",
                TypeKind.Struct => "struct",
                TypeKind.RecordClass => "record class",
                TypeKind.RecordStruct => "record struct",
                _ => "class"
            };

            string modifiersPart = string.Join(" ", Modifiers.Where(modifier => !string.IsNullOrWhiteSpace(modifier)));
            string prefix = string.IsNullOrEmpty(modifiersPart) ? kindKeyword : $"{modifiersPart} {kindKeyword}";

            string constraintsPart = string.Join(" ", TypeParameterConstraintClauses.Where(clause => !string.IsNullOrWhiteSpace(clause)));
            string declaration = $"{prefix} {DeclarationName}".Trim();

            return string.IsNullOrEmpty(constraintsPart) ? declaration : $"{declaration} {constraintsPart}";
        }
    }
}
