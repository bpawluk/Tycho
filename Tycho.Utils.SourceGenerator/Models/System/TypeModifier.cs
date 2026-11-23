using System;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct TypeModifier : IEquatable<TypeModifier>
    {
        public static TypeModifier New { get; } = new TypeModifier("new");
        public static TypeModifier Public { get; } = new TypeModifier("public");
        public static TypeModifier Protected { get; } = new TypeModifier("protected");
        public static TypeModifier Internal { get; } = new TypeModifier("internal");
        public static TypeModifier Private { get; } = new TypeModifier("private");
        public static TypeModifier File { get; } = new TypeModifier("file");
        public static TypeModifier ProtectedInternal { get; } = new TypeModifier("protected internal");
        public static TypeModifier PrivateProtected { get; } = new TypeModifier("private protected");
        public static TypeModifier Static { get; } = new TypeModifier("static");
        public static TypeModifier Virtual { get; } = new TypeModifier("virtual");
        public static TypeModifier Sealed { get; } = new TypeModifier("sealed");
        public static TypeModifier Override { get; } = new TypeModifier("override");
        public static TypeModifier Abstract { get; } = new TypeModifier("abstract");
        public static TypeModifier Extern { get; } = new TypeModifier("extern");
        public static TypeModifier Const { get; } = new TypeModifier("const");
        public static TypeModifier Event { get; } = new TypeModifier("event");
        public static TypeModifier Fixed { get; } = new TypeModifier("fixed");
        public static TypeModifier ReadOnly { get; } = new TypeModifier("readonly");
        public static TypeModifier Ref { get; } = new TypeModifier("ref");
        public static TypeModifier In { get; } = new TypeModifier("in");
        public static TypeModifier Out { get; } = new TypeModifier("out");
        public static TypeModifier Params { get; } = new TypeModifier("params");
        public static TypeModifier This { get; } = new TypeModifier("this");
        public static TypeModifier Scoped { get; } = new TypeModifier("scoped");
        public static TypeModifier Unsafe { get; } = new TypeModifier("unsafe");
        public static TypeModifier Volatile { get; } = new TypeModifier("volatile");
        public static TypeModifier Async { get; } = new TypeModifier("async");
        public static TypeModifier Partial { get; } = new TypeModifier("partial");
        public static TypeModifier Required { get; } = new TypeModifier("required");

        public string Keyword { get; }

        private TypeModifier(string keyword)
        {
            Keyword = keyword ?? string.Empty;
        }

        public bool Equals(TypeModifier other)
        {
            return string.Equals(Keyword, other.Keyword, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is TypeModifier other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Keyword);
        }

        public override string ToString() => Keyword;

        public static bool operator ==(TypeModifier left, TypeModifier right) => left.Equals(right);

        public static bool operator !=(TypeModifier left, TypeModifier right) => !left.Equals(right);
    }
}
