using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Tycho.Utils.SourceGenerator.Extractors;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Extractors;

public sealed class MethodInvokationExtractorTests
{
    [Fact]
    public void MethodInvokationExtractor_ExtractsGenericArguments()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            """
            namespace Demo;
            public static class Extensions
            {
                public static T Echo<T>(this T value) => value;
            }
            public class Calls
            {
                public int CallExtension(int value) => value.Echo<int>();
            }
            """);

        InvocationExpressionSyntax invocationSyntax = compilation.Compilation.SyntaxTrees.Single()
            .GetRoot(TestContext.Current.CancellationToken)
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Single();

        SemanticModel semanticModel = compilation.Compilation.GetSemanticModel(invocationSyntax.SyntaxTree);
        IMethodSymbol invokedMethod = (IMethodSymbol)semanticModel.GetSymbolInfo(
            invocationSyntax,
            TestContext.Current.CancellationToken).Symbol!;

        // Act
        MethodInvocationModel result = MethodInvokationExtractor.Extract(invokedMethod, compilation.Context);

        // Assert
        TypeArgumentModel item = Assert.Single(result.TypeArguments);
        Assert.Equal("T", item.Name);
        Assert.Equal("Echo", result.Signature.MethodName);
        Assert.Equal("global::System.Int32", item.Value.FullReferenceName);
    }

    [Fact]
    public void MethodInvokationExtractor_ExtractsExtensionReceiver()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            """
            namespace Demo;
            public static class Extensions
            {
                public static T Echo<T>(this T value) => value;
            }
            public class Calls
            {
                public int CallExtension(int value) => value.Echo<int>();
            }
            """);

        InvocationExpressionSyntax invocationSyntax = compilation.Compilation.SyntaxTrees.Single()
            .GetRoot(TestContext.Current.CancellationToken)
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Single();

        SemanticModel semanticModel = compilation.Compilation.GetSemanticModel(invocationSyntax.SyntaxTree);
        IMethodSymbol invokedMethod = (IMethodSymbol)semanticModel.GetSymbolInfo(
            invocationSyntax,
            TestContext.Current.CancellationToken).Symbol!;

        // Act
        MethodInvocationModel result = MethodInvokationExtractor.Extract(invokedMethod, compilation.Context);

        // Assert
        Assert.True(result.ReceiverType.HasValue);
        Assert.Equal("global::System.Int32", result.ReceiverType!.Value.FullReferenceName);
    }

    [Fact]
    public void MethodInvokationExtractor_HandlesStaticMethodWithoutTypeArguments()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            "namespace Demo; public class Calls { public static void StaticMethod() { } }");
        IMethodSymbol method = compilation.Method("Demo.Calls", "StaticMethod");

        // Act
        MethodInvocationModel result = MethodInvokationExtractor.Extract(method, compilation.Context);

        // Assert
        Assert.Equal("StaticMethod", result.Signature.MethodName);
        Assert.Empty(result.TypeArguments);
        Assert.Equal("global::Demo.Calls", result.ReceiverType?.FullReferenceName);
    }
}
