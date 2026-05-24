using System;
using System.Linq;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct TypeModel : IEquatable<TypeModel>
    {
        public string Namespace { get; }

        public ImmutableEquatableArray<string> ContainingTypes { get; }

        public ImmutableEquatableArray<string> ContainingMetadataNames { get; }

        public ImmutableEquatableArray<string> ContainingTypeDeclarations { get; }

        public ImmutableEquatableArray<string> ContainingTypeDeclarationSignatures { get; }

        public ImmutableEquatableArray<string> ContainingTypeReferences { get; }

        public string Name { get; }

        public string MetadataName { get; }

        public int Arity { get; }

        public string NameWithArity { get; }

        public string TypeParametersSuffix { get; }

        public string TypeArgumentsSuffix { get; }

        public string DeclarationName { get; }

        public string DeclarationConstraints { get; }

        public string DeclarationSignature { get; }

        public string ReferenceName { get; }

        public string DeclarationPath { get; }

        public string DeclarationPathName { get; }

        public string Path { get; }

        public string PathName { get; }

        public string HintName { get; }

        public string FullName { get; }

        public ImmutableEquatableArray<string> TypeParameterConstraintClauses { get; }

        public TypeModel(
            string typeNamespace,
            ImmutableEquatableArray<string> containingTypes,
            string typeName)
            : this(
                typeNamespace,
                containingTypes,
                containingTypes,
                containingTypes,
                containingTypes,
                containingTypes,
                typeName,
                typeName,
                0,
                string.Empty,
                string.Empty,
                ImmutableEquatableArray<string>.Empty)
        {
        }

        public TypeModel(
            string typeNamespace,
            ImmutableEquatableArray<string> containingTypes,
            ImmutableEquatableArray<string> containingMetadataNames,
            ImmutableEquatableArray<string> containingTypeDeclarations,
            ImmutableEquatableArray<string> containingTypeDeclarationSignatures,
            ImmutableEquatableArray<string> containingTypeReferences,
            string typeName,
            string metadataName,
            int arity,
            string typeParametersSuffix,
            string typeArgumentsSuffix,
            ImmutableEquatableArray<string> typeParameterConstraintClauses)
        {
            Namespace = string.IsNullOrWhiteSpace(typeNamespace) ? string.Empty : typeNamespace;
            ContainingTypes = containingTypes;
            ContainingMetadataNames = containingMetadataNames;
            ContainingTypeDeclarations = containingTypeDeclarations;
            ContainingTypeDeclarationSignatures = containingTypeDeclarationSignatures.Count == 0
                ? containingTypeDeclarations
                : containingTypeDeclarationSignatures;
            ContainingTypeReferences = containingTypeReferences;
            Name = typeName;
            MetadataName = string.IsNullOrWhiteSpace(metadataName) ? typeName : metadataName;
            Arity = arity;
            NameWithArity = Arity > 0 ? $"{Name}{Arity}" : Name;
            TypeParametersSuffix = typeParametersSuffix ?? string.Empty;
            TypeArgumentsSuffix = string.IsNullOrEmpty(typeArgumentsSuffix) ? TypeParametersSuffix : typeArgumentsSuffix;
            TypeParameterConstraintClauses = typeParameterConstraintClauses;
            DeclarationName = $"{Name}{TypeParametersSuffix}";
            DeclarationConstraints = TypeParameterConstraintClauses.Count == 0
                ? string.Empty
                : string.Join(" ", TypeParameterConstraintClauses.Where(clause => !string.IsNullOrWhiteSpace(clause)));
            DeclarationSignature = string.IsNullOrEmpty(DeclarationConstraints) ? DeclarationName : $"{DeclarationName} {DeclarationConstraints}";
            ReferenceName = $"{Name}{TypeArgumentsSuffix}";

            DeclarationPath = containingTypeDeclarations.Count == 0
                ? string.Empty
                : string.Join(".", containingTypeDeclarations.Where(containingType => !string.IsNullOrWhiteSpace(containingType)));

            Path = containingTypeReferences.Count == 0
                ? string.Empty
                : string.Join(".", containingTypeReferences.Where(containingType => !string.IsNullOrWhiteSpace(containingType)));

            DeclarationPathName = string.IsNullOrEmpty(DeclarationPath) ? DeclarationName : $"{DeclarationPath}.{DeclarationName}";
            PathName = string.IsNullOrEmpty(Path) ? ReferenceName : $"{Path}.{ReferenceName}";
            FullName = string.IsNullOrEmpty(Namespace) ? PathName : $"{Namespace}.{PathName}";

            string hintPath = containingMetadataNames.Count == 0
                ? MetadataName
                : $"{string.Join(".", containingMetadataNames.Where(containingType => !string.IsNullOrWhiteSpace(containingType)))}.{MetadataName}";
            HintName = string.IsNullOrEmpty(Namespace) ? hintPath : $"{Namespace}.{hintPath}";
        }

        public bool Equals(TypeModel other)
        {
            return string.Equals(Namespace, other.Namespace, StringComparison.Ordinal)
                && ContainingTypes.Equals(other.ContainingTypes)
                && string.Equals(Name, other.Name, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is TypeModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                StringComparer.Ordinal.GetHashCode(Namespace ?? string.Empty),
                ContainingTypes.GetHashCode(),
                StringComparer.Ordinal.GetHashCode(Name ?? string.Empty));
        }

        public override string ToString() => FullName;

        public static bool operator ==(TypeModel left, TypeModel right) => left.Equals(right);

        public static bool operator !=(TypeModel left, TypeModel right) => !left.Equals(right);
    }
}
