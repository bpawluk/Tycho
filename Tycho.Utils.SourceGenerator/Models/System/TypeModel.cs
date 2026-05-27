using System;
using System.Collections.Generic;
using System.Linq;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct TypeModel : IEquatable<TypeModel>
    {
        public TypeKind Kind { get; }

        public string Namespace { get; }

        public ImmutableEquatableArray<TypeModel> ContainingTypes { get; }

        public string Name { get; }

        public ImmutableEquatableArray<TypeModifier> Modifiers { get; }

        public ImmutableEquatableArray<TypeParameterModel> TypeParameters { get; }

        public ImmutableEquatableArray<TypeArgumentModel> TypeArguments { get; }

        public string TypeParametersSuffix => BuildTypeSuffix(TypeParameters);

        public string TypeArgumentsSuffix => TypeArguments.Count == 0 ? BuildTypeSuffix(TypeParameters) : BuildTypeSuffix(TypeArguments);

        public string DeclarationName => $"{Name}{TypeParametersSuffix}";

        public string ReferenceName => $"{Name}{TypeArgumentsSuffix}";

        public string MetadataName => TypeParameters.Count > 0 ? $"{Name}`{TypeParameters.Count}" : Name;

        public string FullReferenceName => BuildPath(
            ContainingTypes.Select(type => type.ReferenceName).ToImmutableEquatableArray(),
            ReferenceName);

        public string FullMetadataName => BuildPath(
            ContainingTypes.Select(type => type.MetadataName).ToImmutableEquatableArray(),
            MetadataName,
            Namespace);

        // TODO START

        public ImmutableEquatableArray<string> TypeParameterConstraintClauses => TypeParameters
            .Select(BuildTypeParameterConstraintClause)
            .Where(clause => !string.IsNullOrWhiteSpace(clause))
            .ToImmutableEquatableArray();

        public string DeclarationSignature => BuildTypeDeclaration();

        public ImmutableEquatableArray<string> ContainingTypeDeclarationSignatures => ContainingTypes
            .Select(type => type.DeclarationSignature)
            .ToImmutableEquatableArray();

        // TODO END

        public TypeModel(string typeNamespace, string typeName)
            : this(
                typeNamespace,
                ImmutableEquatableArray<TypeModel>.Empty,
                TypeKind.Class,
                ImmutableEquatableArray<TypeModifier>.Empty,
                typeName,
                ImmutableEquatableArray<TypeParameterModel>.Empty,
                ImmutableEquatableArray<TypeArgumentModel>.Empty)
        {
        }

        public TypeModel(
            string typeNamespace,
            ImmutableEquatableArray<TypeModel> containingTypes,
            TypeKind kind,
            ImmutableEquatableArray<TypeModifier> modifiers,
            string typeName,
            ImmutableEquatableArray<TypeParameterModel> typeParameters,
            ImmutableEquatableArray<TypeArgumentModel> typeArguments)
        {
            Kind = kind;
            Modifiers = modifiers ?? ImmutableEquatableArray<TypeModifier>.Empty;
            Namespace = typeNamespace ?? string.Empty;
            ContainingTypes = containingTypes ?? ImmutableEquatableArray<TypeModel>.Empty;
            Name = typeName ?? string.Empty;
            TypeParameters = typeParameters ?? ImmutableEquatableArray<TypeParameterModel>.Empty;
            TypeArguments = typeArguments ?? ImmutableEquatableArray<TypeArgumentModel>.Empty;
        }

        public bool Equals(TypeModel other)
        {
            return Kind == other.Kind
                && string.Equals(Namespace, other.Namespace, StringComparison.Ordinal)
                && (ContainingTypes ?? ImmutableEquatableArray<TypeModel>.Empty).Equals(other.ContainingTypes ?? ImmutableEquatableArray<TypeModel>.Empty)
                && string.Equals(Name, other.Name, StringComparison.Ordinal)
                && (Modifiers ?? ImmutableEquatableArray<TypeModifier>.Empty).Equals(other.Modifiers ?? ImmutableEquatableArray<TypeModifier>.Empty)
                && (TypeParameters ?? ImmutableEquatableArray<TypeParameterModel>.Empty).Equals(other.TypeParameters ?? ImmutableEquatableArray<TypeParameterModel>.Empty)
                && (TypeArguments ?? ImmutableEquatableArray<TypeArgumentModel>.Empty).Equals(other.TypeArguments ?? ImmutableEquatableArray<TypeArgumentModel>.Empty);
        }

        public override bool Equals(object obj)
        {
            return obj is TypeModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Kind.GetHashCode(),
                StringComparer.Ordinal.GetHashCode(Namespace),
                ContainingTypes.GetHashCode(),
                StringComparer.Ordinal.GetHashCode(Name),
                Modifiers.GetHashCode(),
                TypeParameters.GetHashCode(),
                TypeArguments.GetHashCode());
        }

        public override string ToString() => string.IsNullOrEmpty(Namespace) ? FullReferenceName : $"{Namespace}.{FullReferenceName}";

        public static bool operator ==(TypeModel left, TypeModel right) => left.Equals(right);

        public static bool operator !=(TypeModel left, TypeModel right) => !left.Equals(right);

        private static string BuildTypeSuffix(ImmutableEquatableArray<TypeParameterModel> values)
        {
            return values.Count == 0 ? string.Empty : $"<{string.Join(", ", values.Select(value => value.Name))}>";
        }

        private static string BuildTypeSuffix(ImmutableEquatableArray<TypeArgumentModel> values)
        {
            return values.Count == 0 ? string.Empty : $"<{string.Join(", ", values.Select(value => value.Value.ReferenceName))}>";
        }

        private static string BuildTypeParameterConstraintClause(TypeParameterModel typeParameter)
        {
            var constraints = new List<string>();
            foreach (TypeParameterConstraintModel constraint in typeParameter.Constraints)
            {
                string constraintText = constraint.ToString();
                if (!string.IsNullOrWhiteSpace(constraintText))
                {
                    constraints.Add(constraintText);
                }
            }

            if (constraints.Count == 0)
            {
                return string.Empty;
            }

            return $"where {typeParameter.Name} : {string.Join(", ", constraints)}";
        }

        private static string BuildPath(ImmutableEquatableArray<string> containingTypes, string typeName, string namespaceName = null)
        {
            string containingPart = containingTypes.Count == 0 ? string.Empty : string.Join(".", containingTypes.Where(segment => !string.IsNullOrWhiteSpace(segment)));
            string containingAndNamePart = string.IsNullOrEmpty(containingPart) ? typeName : $"{containingPart}.{typeName}";
            return string.IsNullOrEmpty(namespaceName) ? containingAndNamePart : $"{namespaceName}.{containingAndNamePart}";
        }

        private string BuildTypeDeclaration()
        {
            string kindKeyword = Kind.Keyword;

            string modifiersPart = string.Join(" ", Modifiers
                .Select(modifier => modifier.ToString())
                .Where(modifier => !string.IsNullOrWhiteSpace(modifier)));
            string prefix = string.IsNullOrEmpty(modifiersPart) ? kindKeyword : $"{modifiersPart} {kindKeyword}";

            string constraintsPart = string.Join(" ", TypeParameterConstraintClauses.Where(clause => !string.IsNullOrWhiteSpace(clause)));
            string declaration = $"{prefix} {DeclarationName}".Trim();

            return string.IsNullOrEmpty(constraintsPart) ? declaration : $"{declaration} {constraintsPart}";
        }
    }
}
