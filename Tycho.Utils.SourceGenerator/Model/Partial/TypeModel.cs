using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Model.Partial
{
    public readonly struct TypeModel : IEquatable<TypeModel>
    {
        public string TypeNamespace { get; }

        public ImmutableEquatableArray<string> ContainingTypes { get; }

        public string TypeName { get; }

        public TypeModel(
            string typeNamespace,
            ImmutableEquatableArray<string> containingTypes,
            string typeName)
        {
            TypeNamespace = typeNamespace;
            ContainingTypes = containingTypes;
            TypeName = typeName;
        }

        public bool Equals(TypeModel other)
        {
            return string.Equals(TypeNamespace, other.TypeNamespace, StringComparison.Ordinal)
                && ContainingTypes.Equals(other.ContainingTypes)
                && string.Equals(TypeName, other.TypeName, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is TypeModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                StringComparer.Ordinal.GetHashCode(TypeNamespace ?? string.Empty),
                ContainingTypes.GetHashCode(),
                StringComparer.Ordinal.GetHashCode(TypeName ?? string.Empty));
        }

        public override string ToString()
        {
            var namespacePrefix = string.IsNullOrWhiteSpace(TypeNamespace) ? string.Empty : TypeNamespace + ".";
            var containingTypesPrefix = ContainingTypes.Count == 0 ? string.Empty : string.Join(".", ContainingTypes) + ".";
            return $"{namespacePrefix}{containingTypesPrefix}{TypeName}";
        }

        public static bool operator ==(TypeModel left, TypeModel right) => left.Equals(right);

        public static bool operator !=(TypeModel left, TypeModel right) => !left.Equals(right);
    }
}
