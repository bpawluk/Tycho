using System;
using System.Linq;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct TypeModel : IEquatable<TypeModel>
    {
        public string Namespace { get; }

        public ImmutableEquatableArray<ContainingTypeModel> ContainingTypes { get; }

        public string Name { get; }

        public ImmutableEquatableArray<TypeParameterModel> TypeParameters { get; }

        public ImmutableEquatableArray<TypeArgumentModel> TypeArguments { get; }

        public string TypeParametersSuffix => BuildTypeSuffix(TypeParameters);

        public string TypeArgumentsSuffix => BuildTypeSuffix(TypeArguments.Count == 0 ? TypeParameters : TypeArguments);

        public string DeclarationName => $"{Name}{TypeParametersSuffix}";

        public string ReferenceName => $"{Name}{TypeArgumentsSuffix}";

        public string MetadataName => TypeParameters.Count > 0 ? $"{Name}`{TypeParameters.Count}" : Name;

        public ImmutableEquatableArray<string> ContainingTypeDeclarationSignatures => ContainingTypes
            .Select(type => type.DeclarationSignature)
            .ToImmutableEquatableArray();

        public string FullReferenceName => BuildPath(
            ContainingTypes.Select(type => type.ReferenceName).ToImmutableEquatableArray(),
            ReferenceName);

        public string FullMetadataName => BuildPath(
            ContainingTypes.Select(type => type.MetadataName).ToImmutableEquatableArray(),
            MetadataName,
            Namespace);

        public TypeModel(string typeNamespace, string typeName)
            : this(
                typeNamespace,
                ImmutableEquatableArray<ContainingTypeModel>.Empty,
                typeName,
                ImmutableEquatableArray<TypeParameterModel>.Empty,
                ImmutableEquatableArray<TypeArgumentModel>.Empty)
        {
        }

        public TypeModel(
            string typeNamespace,
            ImmutableEquatableArray<ContainingTypeModel> containingTypes,
            string typeName,
            ImmutableEquatableArray<TypeParameterModel> typeParameters,
            ImmutableEquatableArray<TypeArgumentModel> typeArguments)
        {
            Namespace = typeNamespace ?? string.Empty;
            ContainingTypes = containingTypes ?? ImmutableEquatableArray<ContainingTypeModel>.Empty;
            Name = typeName;
            TypeParameters = typeParameters ?? ImmutableEquatableArray<TypeParameterModel>.Empty;
            TypeArguments = typeArguments ?? ImmutableEquatableArray<TypeArgumentModel>.Empty;
        }

        public bool Equals(TypeModel other)
        {
            return string.Equals(Namespace, other.Namespace, StringComparison.Ordinal)
                && ContainingTypes.Equals(other.ContainingTypes)
                && string.Equals(Name, other.Name, StringComparison.Ordinal)
                && TypeParameters.Equals(other.TypeParameters)
                && TypeArguments.Equals(other.TypeArguments);
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
                StringComparer.Ordinal.GetHashCode(Name ?? string.Empty),
                TypeParameters.GetHashCode(),
                TypeArguments.GetHashCode());
        }

        public override string ToString() => string.IsNullOrEmpty(Namespace) ? FullReferenceName : $"{Namespace}.{FullReferenceName}";

        public static bool operator ==(TypeModel left, TypeModel right) => left.Equals(right);

        public static bool operator !=(TypeModel left, TypeModel right) => !left.Equals(right);

        private static string BuildTypeSuffix(ImmutableEquatableArray<string> values)
        {
            return values.Count == 0 ? string.Empty : $"<{string.Join(", ", values)}>";
        }

        private static string BuildPath(ImmutableEquatableArray<string> containingTypes, string typeName, string namespaceName = null)
        {
            string containingPart = containingTypes.Count == 0 ? string.Empty : string.Join(".", containingTypes.Where(segment => !string.IsNullOrWhiteSpace(segment)));
            string containingAndNamePart = string.IsNullOrEmpty(containingPart) ? typeName : $"{containingPart}.{typeName}";
            return string.IsNullOrEmpty(namespaceName) ? containingAndNamePart : $"{namespaceName}.{containingAndNamePart}";
        }
    }
}
