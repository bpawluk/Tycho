namespace Tycho.Utils.SourceGenerator.Models.System
{
    public enum TypeParameterConstraintKind
    {
        /// <summary> class </summary>
        ReferenceType,

        /// <summary> class? </summary>
        NullableReferenceType,

        /// <summary> struct </summary>
        ValueType,

        /// <summary> unmanaged </summary>
        Unmanaged,

        /// <summary> notnull </summary>
        NotNull,

        /// <summary> new() </summary>
        Constructor,

        /// <summary> allows ref struct</summary>
        AllowsRefStruct,

        /// <summary> T : Type </summary>
        TypeConstraint
    }
}
