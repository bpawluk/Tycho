using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests._Utils;

internal static class ModelHelpers
{
    public static ImmutableEquatableArray<T> Items<T>(params T[] values) where T : IEquatable<T> => new(values);

    public static TypeReferenceModel TypeReference(
        string name,
        string typeNamespace = "Example") =>
        new(typeNamespace, name);

    public static TypeDefinitionModel TypeDefinition(
        string name,
        string typeNamespace = "Example",
        ImmutableEquatableArray<TypeDefinitionModel>? containingTypes = null,
        TypeKind? kind = null,
        ImmutableEquatableArray<TypeModifier>? modifiers = null,
        ImmutableEquatableArray<TypeParameterModel>? typeParameters = null) =>
        new(
            typeNamespace,
            containingTypes,
            kind ?? TypeKind.Class,
            modifiers,
            name,
            typeParameters);

    public static MethodSignatureModel MethodSignature(
        string name = "Run",
        ImmutableEquatableArray<TypeReferenceModel>? parameters = null,
        TypeReferenceModel? result = null) =>
        new(name, parameters, result ?? TypeReference("Result"));

    public static void AssertValueSemantics<T>(T expected, T equal, T different)
        where T : IEquatable<T>
    {
        Assert.True(expected.Equals(equal));
        Assert.True(expected.Equals((object)equal));
        Assert.Equal(expected.GetHashCode(), equal.GetHashCode());
        Assert.False(expected.Equals(different));
        Assert.False(expected.Equals(new object()));
    }
}
