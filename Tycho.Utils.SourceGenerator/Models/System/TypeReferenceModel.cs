using System;
using System.Linq;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct TypeReferenceModel : IEquatable<TypeReferenceModel>
    {
        public string Namespace { get; }

        public ImmutableEquatableArray<TypeReferenceModel> ContainingTypes { get; }

        public string Name { get; }

        public ImmutableEquatableArray<TypeArgumentModel> TypeArguments { get; }

        public string TypeArgumentsSuffix => BuildTypeSuffix(TypeArguments);

        public string ReferenceName => $"{Name}{TypeArgumentsSuffix}";

        public string FullReferenceName => BuildPath(
            ContainingTypes.Select(type => type.ReferenceName).ToImmutableEquatableArray(),
            ReferenceName);

        public TypeReferenceModel(string typeNamespace, string typeName) : this(
            typeNamespace,
            ImmutableEquatableArray<TypeReferenceModel>.Empty,
            typeName,
            ImmutableEquatableArray<TypeArgumentModel>.Empty)
        {
        }

        public TypeReferenceModel(
            string typeNamespace,
            ImmutableEquatableArray<TypeReferenceModel> containingTypes,
            string typeName,
            ImmutableEquatableArray<TypeArgumentModel> typeArguments)
        {
            Namespace = typeNamespace ?? string.Empty;
            ContainingTypes = containingTypes ?? ImmutableEquatableArray<TypeReferenceModel>.Empty;
            Name = typeName ?? string.Empty;
            TypeArguments = typeArguments ?? ImmutableEquatableArray<TypeArgumentModel>.Empty;
        }

        public bool Equals(TypeReferenceModel other)
        {
            return string.Equals(Namespace, other.Namespace, StringComparison.Ordinal)
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
                ContainingTypes.GetHashCode(),
                StringComparer.Ordinal.GetHashCode(Name),
                TypeArguments.GetHashCode());
        }

        public override string ToString() => string.IsNullOrEmpty(Namespace) ? FullReferenceName : $"{Namespace}.{FullReferenceName}";

        public static bool operator ==(TypeReferenceModel left, TypeReferenceModel right) => left.Equals(right);

        public static bool operator !=(TypeReferenceModel left, TypeReferenceModel right) => !left.Equals(right);

        private static string BuildTypeSuffix(ImmutableEquatableArray<TypeArgumentModel> typeArguments)
        {
            return typeArguments.Count == 0 ? string.Empty : $"<{string.Join(", ", typeArguments.Select(value => value.Value.ReferenceName))}>";
        }

        private static string BuildPath(ImmutableEquatableArray<string> containingTypes, string typeName)
        {
            string containingPart = containingTypes.Count == 0 ? string.Empty : string.Join(".", containingTypes.Where(segment => !string.IsNullOrWhiteSpace(segment)));
            return string.IsNullOrEmpty(containingPart) ? typeName : $"{containingPart}.{typeName}";
        }
    }
}
