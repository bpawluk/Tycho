using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Tycho.Utils.SourceGenerator.Extractors;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Extractors;

public sealed class MethodInvocationExtractorsTests
{
    private static readonly string[] s_expected = ["Local", "Root", "Step"];

    [Fact]
    public void MethodInvokationsExtractor_VisitsBlockExpressionAndLocalFunctionCallsOnce()
    {
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            """
            namespace Demo;
            public class Calls
            {
                public void Root()
                {
                    Step();
                    void Local() => Step();
                    Local();
                }

                public void Step() => Root();
                public void ExpressionBody() => Step();
            }
            """);

        ImmutableEquatableArray<MethodInvocationModel> rootResult = MethodInvokationsExtractor.Extract(
            compilation.Method("Demo.Calls", "Root"),
            compilation.Context);
        ImmutableEquatableArray<MethodInvocationModel> expressionResult = MethodInvokationsExtractor.Extract(
            compilation.Method("Demo.Calls", "ExpressionBody"),
            compilation.Context);

        string[] rootMethodNames = [.. rootResult.Select(invocation => invocation.Signature.MethodName)];
        Assert.Contains("Step", rootMethodNames);
        Assert.Contains("Local", rootMethodNames);
        Assert.Equal(rootMethodNames.Length, rootMethodNames.Distinct(StringComparer.Ordinal).Count());
        Assert.Contains("Root", rootMethodNames);
        Assert.Equal("Step", expressionResult[0].Signature.MethodName);
        Assert.Equal(
            s_expected,
            expressionResult.Select(invocation => invocation.Signature.MethodName)
                .OrderBy(name => name, StringComparer.Ordinal));
    }

    [Fact]
    public void MethodInvokationsExtractor_UsesUniqueCandidateForUnresolvedCall()
    {
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            """
            namespace Demo;
            public class Calls
            {
                public void Only(int value) { }
                public void Root() => Only("wrong argument type");
            }
            """);

        ImmutableEquatableArray<MethodInvocationModel> result = MethodInvokationsExtractor.Extract(
            compilation.Method("Demo.Calls", "Root"),
            compilation.Context);

        Assert.Contains(result, invocation => invocation.Signature.MethodName == "Only");
    }

    [Fact]
    public void MethodInvokationsExtractor_ResolvesDelegateInvocationSymbol()
    {
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            """
            using System;
            namespace Demo;
            public class Calls
            {
                private readonly Action _callback = () => { };
                public void Root() => _callback();
            }
            """);

        ImmutableEquatableArray<MethodInvocationModel> result = MethodInvokationsExtractor.Extract(
            compilation.Method("Demo.Calls", "Root"),
            compilation.Context);

        Assert.Contains(result, invocation => invocation.Signature.MethodName == "Invoke");
    }

    [Fact]
    public void MethodInvokationsExtractor_IgnoresAmbiguousAndUnresolvedCalls()
    {
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            """
            namespace Demo;
            public class Calls
            {
                public void Choose(string value) { }
                public void Choose(System.Uri value) { }
                public void Root() => Choose(null);
            }
            """);

        ImmutableEquatableArray<MethodInvocationModel> result = MethodInvokationsExtractor.Extract(
            compilation.Method("Demo.Calls", "Root"),
            compilation.Context);

        Assert.Empty(result);
    }

    [Fact]
    public void MethodInvokationsExtractor_SkipsMetadataAndForeignSyntaxDeclarations()
    {
        RoslynTestCompilation sourceCompilation = RoslynTestCompilation.Create(
            "namespace Demo; public class Calls { public void Root() { } }");
        RoslynTestCompilation foreignCompilation = RoslynTestCompilation.Create(
            "namespace Demo; public class Foreign { public void Root() { } }");

        ImmutableEquatableArray<MethodInvocationModel> foreignResult = MethodInvokationsExtractor.Extract(
            foreignCompilation.Method("Demo.Foreign", "Root"),
            sourceCompilation.Context);
        ImmutableEquatableArray<MethodInvocationModel> constructorResult = MethodInvokationsExtractor.Extract(
            sourceCompilation.Type("Demo.Calls").InstanceConstructors.Single(),
            sourceCompilation.Context);

        Assert.Empty(foreignResult);
        Assert.Empty(constructorResult);
    }

    [Fact]
    public void MethodInvokationExtractor_ExtractsGenericArgumentsAndExtensionReceiver()
    {
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            """
            namespace Demo;
            public static class Extensions
            {
                public static T Echo<T>(this T value) => value;
            }
            public class Calls
            {
                public int Root(int value) => value.Echo<int>();
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

        MethodInvocationModel result = MethodInvokationExtractor.Extract(invokedMethod, compilation.Context);

        Assert.Equal("Echo", result.Signature.MethodName);
        Assert.Equal("T", Assert.Single(result.TypeArguments).Name);
        Assert.Equal("global::System.Int32", Assert.Single(result.TypeArguments).Value.FullReferenceName);
        Assert.True(result.ReceiverType.HasValue);
        Assert.Equal("global::System.Int32", result.ReceiverType!.Value.FullReferenceName);
    }

    [Fact]
    public void MethodInvokationExtractor_HandlesStaticMethodWithoutTypeArguments()
    {
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            "namespace Demo; public class Calls { public static void Run() { } }");
        IMethodSymbol method = compilation.Method("Demo.Calls", "Run");

        MethodInvocationModel result = MethodInvokationExtractor.Extract(method, compilation.Context);

        Assert.Equal("Run", result.Signature.MethodName);
        Assert.Empty(result.TypeArguments);
        Assert.Equal("global::Demo.Calls", result.ReceiverType?.FullReferenceName);
    }
}
