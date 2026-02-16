using System;
using System.Linq;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct TypeModel : IEquatable<TypeModel>
    {
        public string Namespace { get; }

        public ImmutableEquatableArray<string> ContainingTypes { get; }

        public string Name { get; }

        public string Path { get; }

        public string PathName { get; }

        public string FullName { get; }

        public TypeModel(
            string typeNamespace,
            ImmutableEquatableArray<string> containingTypes,
            string typeName)
        {
            Namespace = string.IsNullOrWhiteSpace(typeNamespace) ? string.Empty : typeNamespace;
            ContainingTypes = containingTypes;
            Name = typeName;

            Path = containingTypes.Count == 0 ? string.Empty : string.Join(".", containingTypes.Where(containingType => !string.IsNullOrWhiteSpace(containingType)));
            PathName = string.IsNullOrEmpty(Path) ? Name : $"{Path}.{Name}";
            FullName = string.IsNullOrEmpty(Namespace) ? PathName : $"{Namespace}.{PathName}";
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
