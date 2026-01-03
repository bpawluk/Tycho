using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Model.Partial
{
    public readonly struct TypeModel : IEquatable<TypeModel>
    {
        public string TypeNamespace { get; }

        public string TypeName { get; }

        public TypeModel(
            string typeNamespace,
            string typeName)
        {
            TypeNamespace = typeNamespace;
            TypeName = typeName;
        }

        public bool Matches(string fullTypeName)
        {
            var expectedFullName = string.IsNullOrWhiteSpace(TypeNamespace) ? TypeName : $"{TypeNamespace}.{TypeName}";
            return string.Equals(expectedFullName, fullTypeName, StringComparison.Ordinal);
        }

        public bool Equals(TypeModel other)
        {
            return string.Equals(TypeNamespace, other.TypeNamespace, StringComparison.Ordinal)
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
                StringComparer.Ordinal.GetHashCode(TypeName ?? string.Empty));
        }

        public static bool operator ==(TypeModel left, TypeModel right) => left.Equals(right);

        public static bool operator !=(TypeModel left, TypeModel right) => !left.Equals(right);
    }
}
