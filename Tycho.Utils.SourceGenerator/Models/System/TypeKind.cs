using System;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct TypeKind : IEquatable<TypeKind>
    {
        public static TypeKind Class { get; } = new TypeKind("class");
        public static TypeKind Interface { get; } = new TypeKind("interface");
        public static TypeKind Struct { get; } = new TypeKind("struct");
        public static TypeKind RecordClass { get; } = new TypeKind("record class");
        public static TypeKind RecordStruct { get; } = new TypeKind("record struct");

        public string Keyword { get; }

        private TypeKind(string keyword)
        {
            Keyword = keyword ?? string.Empty;
        }

        public bool Equals(TypeKind other)
        {
            return string.Equals(Keyword, other.Keyword, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is TypeKind other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Keyword);
        }

        public override string ToString() => Keyword;

        public static bool operator ==(TypeKind left, TypeKind right) => left.Equals(right);

        public static bool operator !=(TypeKind left, TypeKind right) => !left.Equals(right);
    }
}
