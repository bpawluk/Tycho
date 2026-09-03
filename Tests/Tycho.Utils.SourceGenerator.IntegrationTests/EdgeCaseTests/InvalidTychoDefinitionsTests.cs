using Microsoft.CodeAnalysis;
using static Tycho.Utils.SourceGenerator.IntegrationTests._Utils.RunHelpers;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.EdgeCaseTests;

public class InvalidTychoDefinitionsTests
{
    [Fact]
    public void InvalidTychoDefinitions_AreSkippedWithoutAffectingValidDefinitions()
    {
        GeneratorDriver driver = RunGenerator("EdgeCaseTests\\SUT", ["InvalidTychoDefinitions.cs"]);
        GeneratorDriverRunResult result = driver.GetRunResult();

        Assert.All(result.Results, generatorResult => Assert.Null(generatorResult.Exception));
        Assert.Equal(8, result.GeneratedTrees.Length);
        Assert.All(result.GeneratedTrees, tree => Assert.Contains("ValidApp", tree.FilePath, StringComparison.Ordinal));
    }
}
