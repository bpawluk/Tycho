using System;
using System.Linq;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct TypeReferenceModel : IEquatable<TypeReferenceModel>
    {
        public string Namespace { get; }

        public bool IsTypeParameter { get; }

        public ImmutableEquatableArray<TypeReferenceModel> ContainingTypes { get; }

        public string Name { get; }

        public ImmutableEquatableArray<TypeArgumentModel> TypeArguments { get; }

        public string TypeArgumentsSuffix => BuildTypeSuffix(TypeArguments);

        public string ReferenceName => $"{Name}{TypeArgumentsSuffix}";

        public string FullReferenceName
        {
            get
            {
                if (IsTypeParameter)
                {
                    return ReferenceName;
                }

                string path = BuildPath(
                    ContainingTypes.Select(type => type.ReferenceName).ToImmutableEquatableArray(),
                    ReferenceName);
                string qualifiedPath = string.IsNullOrEmpty(Namespace) ? path : $"{Namespace}.{path}";
                return $"global::{qualifiedPath}";
            }
        }

        public TypeReferenceModel(string typeNamespace, string typeName) : this(
            typeNamespace,
            ImmutableEquatableArray<TypeReferenceModel>.Empty,
            typeName,
            ImmutableEquatableArray<TypeArgumentModel>.Empty,
            isTypeParameter: false)
        {
        }

        public TypeReferenceModel(
            string typeNamespace,
            ImmutableEquatableArray<TypeReferenceModel> containingTypes,
            string typeName,
            ImmutableEquatableArray<TypeArgumentModel> typeArguments) : this(
                typeNamespace,
                containingTypes,
                typeName,
                typeArguments,
                isTypeParameter: false)
        {
        }

        public TypeReferenceModel(
            string typeNamespace,
            ImmutableEquatableArray<TypeReferenceModel> containingTypes,
            string typeName,
            ImmutableEquatableArray<TypeArgumentModel> typeArguments,
            bool isTypeParameter)
        {
            Namespace = typeNamespace ?? string.Empty;
            ContainingTypes = containingTypes ?? ImmutableEquatableArray<TypeReferenceModel>.Empty;
            Name = typeName ?? string.Empty;
            TypeArguments = typeArguments ?? ImmutableEquatableArray<TypeArgumentModel>.Empty;
            IsTypeParameter = isTypeParameter;
        }

        public static TypeReferenceModel TypeParameter(string typeNamespace, string typeName) => new TypeReferenceModel(
            typeNamespace,
            ImmutableEquatableArray<TypeReferenceModel>.Empty,
            typeName,
            ImmutableEquatableArray<TypeArgumentModel>.Empty,
            isTypeParameter: true);

        public bool Matches(TypeReferenceModel other)
        {
            return string.Equals(Namespace, other.Namespace, StringComparison.Ordinal)
                && IsTypeParameter == other.IsTypeParameter
                && ContainingTypes.Count == other.ContainingTypes.Count
                && ContainingTypes.Zip(other.ContainingTypes, (type, otherType) => type.Matches(otherType)).All(match => match)
                && string.Equals(Name, other.Name, StringComparison.Ordinal)
                && TypeArguments.Count == other.TypeArguments.Count
                && TypeArguments.Zip(other.TypeArguments, (a, b) => a.Matches(b)).All(match => match);
        }

        public bool Equals(TypeReferenceModel other)
        {
            return string.Equals(Namespace, other.Namespace, StringComparison.Ordinal)
                && IsTypeParameter == other.IsTypeParameter
                && (ContainingTypes ?? ImmutableEquatableArray<TypeReferenceModel>.Empty).Equals(other.ContainingTypes ?? ImmutableEquatableArray<TypeReferenceModel>.Empty)
                && string.Equals(Name, other.Name, StringComparison.Ordinal)
                && (TypeArguments ?? ImmutableEquatableArray<TypeArgumentModel>.Empty).Equals(other.TypeArguments ?? ImmutableEquatableArray<TypeArgumentModel>.Empty);
        }

        public override bool Equals(object obj)
        {
            return obj is TypeReferenceModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                StringComparer.Ordinal.GetHashCode(Namespace),
                IsTypeParameter.GetHashCode(),
                ContainingTypes.GetHashCode(),
                StringComparer.Ordinal.GetHashCode(Name),
                TypeArguments.GetHashCode());
        }

        public override string ToString() => FullReferenceName;

        public static bool operator ==(TypeReferenceModel left, TypeReferenceModel right) => left.Equals(right);

        public static bool operator !=(TypeReferenceModel left, TypeReferenceModel right) => !left.Equals(right);

        private static string BuildTypeSuffix(ImmutableEquatableArray<TypeArgumentModel> typeArguments)
        {
            return typeArguments.Count == 0 ? string.Empty : $"<{string.Join(", ", typeArguments.Select(value => value.Value.FullReferenceName))}>";
        }

        private static string BuildPath(ImmutableEquatableArray<string> containingTypes, string typeName)
        {
            string containingPart = containingTypes.Count == 0 ? string.Empty : string.Join(".", containingTypes.Where(segment => !string.IsNullOrWhiteSpace(segment)));
            return string.IsNullOrEmpty(containingPart) ? typeName : $"{containingPart}.{typeName}";
        }
    }
}
