using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Extractors;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Extractors;

public sealed class TypeExtractorsTests
{
    private static readonly string[] s_protectedSealed = ["protected", "sealed"];
    private static readonly string[] s_privateStatic = ["private", "static"];
    private static readonly string[] s_publicNew = ["public", "new"];
    private static readonly string[] s_protectedInternal = ["protected", "internal"];
    private static readonly string[] s_privateProtected = ["private", "protected"];
    private static readonly string[] s_public = ["public"];
    private static readonly string[] s_singleTypeParameter = ["T"];

    private static readonly RoslynTestCompilation s_compilation = RoslynTestCompilation.Create(
        """
        using System;

        namespace Demo;

        public class PlainType { }
        public record RecordClass { }
        public struct PlainStruct { }
        public record struct RecordStruct { }
        public interface IContract { }
        public enum State { Ready }
        public delegate void Callback();
        file class FileType { }
        public static class StaticType { }
        public abstract class AbstractType { }
        public readonly ref struct ReadonlyRefStruct { }
        public unsafe class UnsafeType { }
        class ImplicitlyInternalType { }
        public partial class PartialType { }
        public partial class PartialType { }
        public class GenericType<T> { }
        public class Outer<T> { public class Inner<U> { } }

        public class ReferenceConstraint<T> where T : class { }
        public class NullableReferenceConstraint<T> where T : class? { }
        public class ValueConstraint<T> where T : struct { }
        public class UnmanagedConstraint<T> where T : unmanaged { }
        public class NotNullConstraint<T> where T : notnull { }
        public class ConstructorConstraint<T> where T : IDisposable, new() { }
        public class TypeConstraint<T> where T : IDisposable { }
        public class AllowsRefStructConstraint<T> where T : allows ref struct { }
        """);

    [Theory]
    [InlineData("Demo.PlainType", "class")]
    [InlineData("Demo.RecordClass", "record class")]
    [InlineData("Demo.PlainStruct", "struct")]
    [InlineData("Demo.RecordStruct", "record struct")]
    [InlineData("Demo.IContract", "interface")]
    [InlineData("Demo.State", "enum")]
    [InlineData("Demo.Callback", "other")]
    public void TypeKindExtractor_MapsTypeDeclarations(string metadataName, string expected)
    {
        ITypeSymbol type = s_compilation.Type(metadataName);

        Tycho.Utils.SourceGenerator.Models.System.TypeKind result = TypeKindExtractor.Extract(type, s_compilation.Context);

        Assert.Equal(expected, result.Keyword);
    }

    [Fact]
    public void TypeKindExtractor_MapsTypeParametersToOther()
    {
        ITypeParameterSymbol typeParameter = s_compilation.TypeParameter("Demo.GenericType`1", "T");

        Tycho.Utils.SourceGenerator.Models.System.TypeKind result = TypeKindExtractor.Extract(typeParameter, s_compilation.Context);

        Assert.Equal(Tycho.Utils.SourceGenerator.Models.System.TypeKind.Other, result);
    }

    [Fact]
    public void TychoDefinitionKindExtractor_RecognizesAppsModulesAndUnknownTypes()
    {
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            """
            namespace Tycho.Apps { public class TychoApp { } }
            namespace Tycho.Modules { public class TychoModule { } }
            namespace Demo
            {
                public class App : Tycho.Apps.TychoApp { }
                public class AppChild : App { }
                public class Module : Tycho.Modules.TychoModule { }
                public class Unrelated { }
            }
            """);

        Assert.Equal(TychoDefinitionKind.App, TychoDefinitionKindExtractor.Extract(
            compilation.Type("Demo.App"), compilation.Context));
        Assert.Equal(TychoDefinitionKind.App, TychoDefinitionKindExtractor.Extract(
            compilation.Type("Demo.AppChild"), compilation.Context));
        Assert.Equal(TychoDefinitionKind.Module, TychoDefinitionKindExtractor.Extract(
            compilation.Type("Demo.Module"), compilation.Context));
        Assert.Equal(TychoDefinitionKind.Unknown, TychoDefinitionKindExtractor.Extract(
            compilation.Type("Demo.Unrelated"), compilation.Context));

        RoslynTestCompilation withoutTychoBases = RoslynTestCompilation.Create("namespace Demo; public class Unrelated { }");
        Assert.Equal(TychoDefinitionKind.Unknown, TychoDefinitionKindExtractor.Extract(
            withoutTychoBases.Type("Demo.Unrelated"), withoutTychoBases.Context));
    }

    [Fact]
    public void TypeModifiersExtractor_CollectsLegalModifiersAndDeduplicatesPartialDeclarations()
    {
        (string Name, string[] Expected)[] cases =
        [
            ("Demo.StaticType", ["public", "static"]),
            ("Demo.AbstractType", ["public", "abstract"]),
            ("Demo.FileType", ["file"]),
            ("Demo.ReadonlyRefStruct", ["public", "readonly", "ref"]),
            ("Demo.UnsafeType", ["public", "unsafe"]),
            ("Demo.PartialType", ["public", "partial"]),
        ];

        foreach ((string name, string[] expected) in cases)
        {
            ImmutableEquatableArray<TypeModifier> result = TypeModifiersExtractor.Extract(
                s_compilation.DeclaredType(name.Split('.').Last()),
                s_compilation.Context);

            Assert.Equal(expected, result.Select(modifier => modifier.Keyword));
        }
    }

    [Fact]
    public void TypeModifiersExtractor_CollectsModifiersFromNestedTypesAndDelegates()
    {
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            """
            namespace Demo;
            public abstract class Outer
            {
                protected sealed class ProtectedNested { }
                private static class PrivateNested { }
                public new class NewNested { }
                protected internal class ProtectedInternalNested { }
                private protected class PrivateProtectedNested { }
                public delegate void NestedCallback();
            }
            """);

        Assert.Equal(
            s_protectedSealed,
            TypeModifiersExtractor.Extract(compilation.Type("Demo.Outer+ProtectedNested"), compilation.Context)
                .Select(modifier => modifier.Keyword));
        Assert.Equal(
            s_privateStatic,
            TypeModifiersExtractor.Extract(compilation.Type("Demo.Outer+PrivateNested"), compilation.Context)
                .Select(modifier => modifier.Keyword));
        Assert.Equal(
            s_publicNew,
            TypeModifiersExtractor.Extract(compilation.Type("Demo.Outer+NewNested"), compilation.Context)
                .Select(modifier => modifier.Keyword));
        Assert.Equal(
            s_protectedInternal,
            TypeModifiersExtractor.Extract(compilation.Type("Demo.Outer+ProtectedInternalNested"), compilation.Context)
                .Select(modifier => modifier.Keyword));
        Assert.Equal(
            s_privateProtected,
            TypeModifiersExtractor.Extract(compilation.Type("Demo.Outer+PrivateProtectedNested"), compilation.Context)
                .Select(modifier => modifier.Keyword));
        Assert.Equal(
            s_public,
            TypeModifiersExtractor.Extract(compilation.Type("Demo.Outer+NestedCallback"), compilation.Context)
                .Select(modifier => modifier.Keyword));
    }

    [Fact]
    public void TypeModifiersExtractor_ReturnsEmptyForTypeWithoutExplicitModifiers()
    {
        ImmutableEquatableArray<TypeModifier> result = TypeModifiersExtractor.Extract(
            s_compilation.Type("Demo.ImplicitlyInternalType"),
            s_compilation.Context);

        Assert.Empty(result);
    }

    [Fact]
    public void TypeReferenceModelExtractor_ExtractsTypeParametersNamedTypesAndArrayTypes()
    {
        ITypeParameterSymbol typeParameter = s_compilation.TypeParameter("Demo.GenericType`1", "T");
        ITypeSymbol nestedGeneric = s_compilation.Type("Demo.Outer`1+Inner`1");
        ITypeSymbol array = s_compilation.Compilation.CreateArrayTypeSymbol(
            s_compilation.Compilation.GetSpecialType(SpecialType.System_Int32));

        TypeReferenceModel parameterResult = TypeReferenceModelExtractor.Extract(typeParameter, s_compilation.Context);
        TypeReferenceModel namedResult = TypeReferenceModelExtractor.Extract(nestedGeneric, s_compilation.Context);
        TypeReferenceModel arrayResult = TypeReferenceModelExtractor.Extract(array, s_compilation.Context);

        Assert.True(parameterResult.IsTypeParameter);
        Assert.Equal("T", parameterResult.FullReferenceName);
        Assert.Equal("global::Demo.Outer<T>.Inner<U>", namedResult.FullReferenceName);
        Assert.Equal(array.Name, arrayResult.Name);
        Assert.Empty(arrayResult.ContainingTypes);
        Assert.Empty(arrayResult.TypeArguments);

        RoslynTestCompilation globalCompilation = RoslynTestCompilation.Create("public class GlobalType { }");
        TypeReferenceModel globalType = TypeReferenceModelExtractor.Extract(
            globalCompilation.Type("GlobalType"),
            globalCompilation.Context);
        Assert.Empty(globalType.Namespace);
    }

    [Fact]
    public void TypeParametersExtractor_ReturnsParametersOnlyForGenericNamedTypes()
    {
        ImmutableEquatableArray<TypeParameterModel> genericResult = TypeParametersExtractor.Extract(
            s_compilation.Type("Demo.GenericType`1"),
            s_compilation.Context);
        ImmutableEquatableArray<TypeParameterModel> nonGenericResult = TypeParametersExtractor.Extract(
            s_compilation.Type("Demo.PlainType"),
            s_compilation.Context);
        ITypeParameterSymbol typeParameter = s_compilation.TypeParameter("Demo.GenericType`1", "T");
        ImmutableEquatableArray<TypeParameterModel> parameterResult = TypeParametersExtractor.Extract(
            typeParameter,
            s_compilation.Context);

        Assert.Equal(s_singleTypeParameter, genericResult.Select(parameter => parameter.Name));
        Assert.Empty(nonGenericResult);
        Assert.Empty(parameterResult);
    }

    [Fact]
    public void TypeArgumentsModelExtractor_HandlesGenericAndNonGenericMethodsAndTypes()
    {
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            """
            namespace Demo;
            public class Box<T> { }
            public class Methods
            {
                public void Generic<T>() { }
                public void NonGeneric() { }
            }
            """);

        ImmutableEquatableArray<TypeArgumentModel> genericMethod = TypeArgumentsModelExtractor.Extract(
            compilation.Method("Demo.Methods", "Generic"),
            compilation.Context);
        ImmutableEquatableArray<TypeArgumentModel> nonGenericMethod = TypeArgumentsModelExtractor.Extract(
            compilation.Method("Demo.Methods", "NonGeneric"),
            compilation.Context);
        ImmutableEquatableArray<TypeArgumentModel> genericType = TypeArgumentsModelExtractor.Extract(
            compilation.Type("Demo.Box`1"),
            compilation.Context);
        ImmutableEquatableArray<TypeArgumentModel> nonGenericType = TypeArgumentsModelExtractor.Extract(
            compilation.Type("Demo.Methods"),
            compilation.Context);

        Assert.Equal(s_singleTypeParameter, genericMethod.Select(argument => argument.Name));
        Assert.Empty(nonGenericMethod);
        Assert.Equal(s_singleTypeParameter, genericType.Select(argument => argument.Name));
        Assert.Empty(nonGenericType);
    }

    public static TheoryData<string, string[]> ConstraintCases => new()
    {
        { "Demo.ReferenceConstraint`1", ["class"] },
        { "Demo.NullableReferenceConstraint`1", ["class?"] },
        { "Demo.ValueConstraint`1", ["struct"] },
        { "Demo.UnmanagedConstraint`1", ["unmanaged"] },
        { "Demo.NotNullConstraint`1", ["notnull"] },
        { "Demo.ConstructorConstraint`1", ["global::System.IDisposable", "new()"] },
        { "Demo.TypeConstraint`1", ["global::System.IDisposable"] },
        { "Demo.AllowsRefStructConstraint`1", ["allows ref struct"] },
    };

    [Theory]
    [MemberData(nameof(ConstraintCases))]
    public void TypeParameterConstraintsExtractor_ExtractsConstraintKinds(string metadataName, string[] expected)
    {
        ITypeParameterSymbol parameter = s_compilation.Type(metadataName).TypeParameters.Single();

        ImmutableEquatableArray<TypeParameterConstraintModel> result = TypeParameterConstraintsExtractor.Extract(
            parameter,
            s_compilation.Context);

        Assert.Equal(expected, result.Select(constraint => constraint.Keyword));
    }
}
