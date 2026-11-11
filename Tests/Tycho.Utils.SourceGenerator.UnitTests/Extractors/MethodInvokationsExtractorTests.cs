using Tycho.Utils.SourceGenerator.Extractors;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.UnitTests._Utils;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.UnitTests.Extractors;

public sealed class MethodInvokationsExtractorTests
{
    [Fact]
    public void MethodInvokationsExtractor_VisitsBlockBodyAndReachableCallsOnce()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            """
            namespace Demo;
            public class Calls
            {
                public void BlockBody()
                {
                    CallsBlockBody();
                    void LocalFunction() => CallsBlockBody();
                    LocalFunction();
                }

                public void CallsBlockBody() => BlockBody();
            }
            """);

        // Act
        ImmutableEquatableArray<MethodInvocationModel> result = MethodInvokationsExtractor.Extract(
            compilation.Method("Demo.Calls", "BlockBody"),
            compilation.Context);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("CallsBlockBody", result[0].Signature.MethodName);
        Assert.Equal("LocalFunction", result[1].Signature.MethodName);
        Assert.Equal("BlockBody", result[2].Signature.MethodName);
    }

    [Fact]
    public void MethodInvokationsExtractor_VisitsExpressionBodyAndReachableCallsOnce()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            """
            namespace Demo;
            public class Calls
            {
                public void ExpressionBody() => CallsBlockBody();
                public void CallsBlockBody() => BlockBody();

                public void BlockBody()
                {
                    CallsBlockBody();
                    void LocalFunction() => CallsBlockBody();
                    LocalFunction();
                }
            }
            """);

        // Act
        ImmutableEquatableArray<MethodInvocationModel> result = MethodInvokationsExtractor.Extract(
            compilation.Method("Demo.Calls", "ExpressionBody"),
            compilation.Context);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("CallsBlockBody", result[0].Signature.MethodName);
        Assert.Equal("BlockBody", result[1].Signature.MethodName);
        Assert.Equal("LocalFunction", result[2].Signature.MethodName);
    }

    [Fact]
    public void MethodInvokationsExtractor_UsesUniqueCandidateForUnresolvedCall()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            """
            namespace Demo;
            public class Calls
            {
                public void OnlyCandidate(int value) { }
                public void CallWithWrongArgumentType() => OnlyCandidate("wrong argument type");
            }
            """);

        // Act
        ImmutableEquatableArray<MethodInvocationModel> result = MethodInvokationsExtractor.Extract(
            compilation.Method("Demo.Calls", "CallWithWrongArgumentType"),
            compilation.Context);

        // Assert
        MethodInvocationModel item = Assert.Single(result);
        Assert.Equal("OnlyCandidate", item.Signature.MethodName);
    }

    [Fact]
    public void MethodInvokationsExtractor_ResolvesDelegateInvocationSymbol()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            """
            using System;
            namespace Demo;
            public class Calls
            {
                private readonly Action _callback = () => { };
                public void InvokeCallback() => _callback();
            }
            """);

        // Act
        ImmutableEquatableArray<MethodInvocationModel> result = MethodInvokationsExtractor.Extract(
            compilation.Method("Demo.Calls", "InvokeCallback"),
            compilation.Context);

        // Assert
        MethodInvocationModel item = Assert.Single(result);
        Assert.Equal("Invoke", item.Signature.MethodName);
    }

    [Fact]
    public void MethodInvokationsExtractor_IgnoresAmbiguousCalls()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            """
            namespace Demo;
            public class Calls
            {
                public void Choose(string value) { }
                public void Choose(System.Uri value) { }
                public void CallAmbiguousOverload() => Choose(null);
            }
            """);

        // Act
        ImmutableEquatableArray<MethodInvocationModel> result = MethodInvokationsExtractor.Extract(
            compilation.Method("Demo.Calls", "CallAmbiguousOverload"),
            compilation.Context);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void MethodInvokationsExtractor_SkipsForeignSyntaxDeclarations()
    {
        // Arrange
        RoslynTestCompilation sourceCompilation = RoslynTestCompilation.Create(
            "namespace Demo; public class Calls { public void SourceMethod() { } }");
        RoslynTestCompilation foreignCompilation = RoslynTestCompilation.Create(
            "namespace Demo; public class Foreign { public void ForeignMethod() { } }");

        // Act
        ImmutableEquatableArray<MethodInvocationModel> result = MethodInvokationsExtractor.Extract(
            foreignCompilation.Method("Demo.Foreign", "ForeignMethod"),
            sourceCompilation.Context);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void MethodInvokationsExtractor_SkipsImplicitConstructorWithoutSyntaxDeclaration()
    {
        // Arrange
        RoslynTestCompilation compilation = RoslynTestCompilation.Create(
            "namespace Demo; public class Calls { }");

        // Act
        ImmutableEquatableArray<MethodInvocationModel> result = MethodInvokationsExtractor.Extract(
            compilation.Type("Demo.Calls").InstanceConstructors.Single(),
            compilation.Context);

        // Assert
        Assert.Empty(result);
    }
}
