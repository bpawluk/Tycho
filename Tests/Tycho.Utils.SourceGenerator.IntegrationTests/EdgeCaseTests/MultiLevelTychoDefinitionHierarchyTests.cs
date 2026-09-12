using Microsoft.CodeAnalysis;
using static Tycho.Utils.SourceGenerator.IntegrationTests._Utils.RunHelpers;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.EdgeCaseTests;

public class MultiLevelTychoDefinitionHierarchyTests
{
    [Fact]
    public void MultiLevelTychoDefinitionHierarchy_DoesNotFailTheGenerator()
    {
        GeneratorDriver driver = RunGenerator("EdgeCaseTests\\SUT", ["MultiLevelTychoDefinitions.cs"]);
        GeneratorDriverRunResult result = driver.GetRunResult();

        Assert.All(result.Results, generatorResult => Assert.Null(generatorResult.Exception));
        Assert.Equal(16, result.GeneratedTrees.Length);
    }
}
