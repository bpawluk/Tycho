using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Tycho.Utils.SourceGenerator.IntegrationTests._Utils.RunHelpers;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.EdgeCaseTests;

public class InvalidTychoDefinitionsTests
{
    [Fact]
    public void InvalidDefinitionsAreSkippedWithoutAffectingValidDefinitions()
    {
        GeneratorDriver driver = RunGenerator("SnapshotTests\\Input", ["InvalidDefinitions/Definitions.cs"]);
        GeneratorDriverRunResult result = driver.GetRunResult();

        Assert.All(result.Results, generatorResult => Assert.Null(generatorResult.Exception));
        Assert.Equal(8, result.GeneratedTrees.Length);
        Assert.All(result.GeneratedTrees, tree => Assert.Contains("ValidApp", tree.FilePath, StringComparison.Ordinal));
    }

    [Fact]
    public void InvalidDefinitionsRemainComparableAcrossIncrementalRuns()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        CSharpCompilation compilation = CreateCompilation("SnapshotTests\\Input", ["InvalidDefinitions/Definitions.cs"]);
        GeneratorDriver driver = CreateGeneratorDriverWithTracking();
        driver = driver.RunGenerators(compilation, cancellationToken);

        SyntaxTree syntaxTree = compilation.SyntaxTrees.Single();
        string updatedSource = syntaxTree.GetText(cancellationToken).ToString().Replace(
            "public class UnrelatedClass { }",
            "public class UnrelatedClass { /* incremental edit */ }",
            StringComparison.Ordinal);
        SyntaxTree updatedSyntaxTree = CSharpSyntaxTree.ParseText(
            updatedSource,
            path: syntaxTree.FilePath,
            cancellationToken: cancellationToken);
        compilation = compilation.ReplaceSyntaxTree(syntaxTree, updatedSyntaxTree);

        driver = driver.RunGenerators(compilation, cancellationToken);
        GeneratorDriverRunResult result = driver.GetRunResult();

        Assert.All(result.Results, generatorResult => Assert.Null(generatorResult.Exception));
        Assert.Equal(8, result.GeneratedTrees.Length);
    }
}
