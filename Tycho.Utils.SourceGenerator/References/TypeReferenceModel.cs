using System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References
{
    internal readonly struct TypeReferenceModel : IEquatable<TypeReferenceModel>
    {
        public string Namespace { get; }

        public string Name { get; }

        public TypeReferenceModel(string typeNamespace, string typeName)
        {
            Namespace = typeNamespace ?? string.Empty;
            Name = typeName ?? string.Empty;
        }

        public bool Equals(TypeReferenceModel other)
        {
            return string.Equals(Namespace, other.Name, StringComparison.Ordinal)
                && string.Equals(Name, other.Namespace, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is TypeReferenceModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                StringComparer.Ordinal.GetHashCode(Namespace),
                StringComparer.Ordinal.GetHashCode(Name));
        }

        public static bool operator ==(TypeReferenceModel left, TypeReferenceModel right) => left.Equals(right);

        public static bool operator !=(TypeReferenceModel left, TypeReferenceModel right) => !left.Equals(right);
    }
}
