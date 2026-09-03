using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Tycho.Utils.SourceGenerator.IntegrationTests._Utils.RunHelpers;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.IncrementalTests;

public class IncrementalTests
{
    [Fact]
    public void ContractChanges_InvalidateOnlyAffectedSteps()
    {
        GeneratorDriverRunResult result = RunTrackedEdit(
            "TestApp.cs",
            "protected override void DefineContract(IAppContract app) { }",
            "protected override void DefineContract(IAppContract app) { app.Expects<string>(); }");

        AssertStepReason(result, "TychoDefinition.Type", IncrementalStepRunReason.Unchanged);
        AssertStepReason(result, "TychoDefinition.Contract", IncrementalStepRunReason.Modified);
        AssertStepReason(result, "TychoDefinition.Events", IncrementalStepRunReason.Unchanged);
        AssertStepReason(result, "TychoDefinition.Structure", IncrementalStepRunReason.Unchanged);
        AssertStepReason(result, "TychoFacade.Model", IncrementalStepRunReason.Modified);
        AssertStepReason(result, "TychoPublisher.Model", IncrementalStepRunReason.Cached);
        AssertStepReason(result, "TychoEventSerializer.Model", IncrementalStepRunReason.Cached);
        AssertStepReason(result, "TychoSetup.Model", IncrementalStepRunReason.Cached);
        AssertStepReason(result, "TychoAppBuilder.Model", IncrementalStepRunReason.Cached);
        AssertStepReason(result, "TychoExtensions.Model", IncrementalStepRunReason.Cached);
    }

    [Fact]
    public void EventChanges_InvalidateOnlyAffectedSteps()
    {
        GeneratorDriverRunResult result = RunTrackedEdit(
            "TestApp.cs",
            "app.Expects<OrderCreatedEvent>()",
            "app.Expects<PaymentFailedEvent>()");

        AssertStepReason(result, "TychoDefinition.Type", IncrementalStepRunReason.Unchanged);
        AssertStepReason(result, "TychoDefinition.Contract", IncrementalStepRunReason.Unchanged);
        AssertStepReason(result, "TychoDefinition.Events", IncrementalStepRunReason.Modified);
        AssertStepReason(result, "TychoDefinition.Structure", IncrementalStepRunReason.Unchanged);
        AssertStepReason(result, "TychoAppBuilder.Model", IncrementalStepRunReason.Cached);
        AssertStepReason(result, "TychoExtensions.Model", IncrementalStepRunReason.Cached);
        AssertStepReason(result, "TychoFacade.Model", IncrementalStepRunReason.Cached);
        AssertStepReason(result, "TychoSetup.Model", IncrementalStepRunReason.Cached);
        AssertStepReason(result, "TychoPublisher.Model", IncrementalStepRunReason.Modified);
        AssertStepReason(result, "TychoEventSerializer.Model", IncrementalStepRunReason.Modified);
    }


    [Fact]
    public void Structure_InvalidateOnlyAffectedSteps()
    {
        GeneratorDriverRunResult result = RunTrackedEdit(
            "TestApp.cs",
            "protected override void IncludeModules(IAppStructure app) { }",
            "protected override void IncludeModules(IAppStructure app) { app.Uses<ModuleA>(); }");

        AssertStepReason(result, "TychoDefinition.Type", IncrementalStepRunReason.Unchanged);
        AssertStepReason(result, "TychoDefinition.Contract", IncrementalStepRunReason.Unchanged);
        AssertStepReason(result, "TychoDefinition.Events", IncrementalStepRunReason.Unchanged);
        AssertStepReason(result, "TychoDefinition.Structure", IncrementalStepRunReason.Modified);
        AssertStepReason(result, "TychoSetup.Model", IncrementalStepRunReason.Modified);
        AssertStepReason(result, "TychoFacade.Model", IncrementalStepRunReason.Cached);
        AssertStepReason(result, "TychoPublisher.Model", IncrementalStepRunReason.Cached);
        AssertStepReason(result, "TychoEventSerializer.Model", IncrementalStepRunReason.Cached);
        AssertStepReason(result, "TychoAppBuilder.Model", IncrementalStepRunReason.Cached);
        AssertStepReason(result, "TychoExtensions.Model", IncrementalStepRunReason.Cached);
    }

    [Fact]
    public void DefinitionTypeChanges_InvalidateAllSteps()
    {
        GeneratorDriverRunResult result = RunTrackedEdit(
            "TestApp.cs",
            "public class TestApp : TychoApp",
            "public class RenamedApp : TychoApp");

        AssertStepReason(result, "TychoDefinition.Type", IncrementalStepRunReason.Modified);
        AssertStepReason(result, "TychoDefinition.Contract", IncrementalStepRunReason.Modified);
        AssertStepReason(result, "TychoDefinition.Events", IncrementalStepRunReason.Modified);
        AssertStepReason(result, "TychoDefinition.Structure", IncrementalStepRunReason.Modified);
        AssertStepReason(result, "TychoFacade.Model", IncrementalStepRunReason.Modified);
        AssertStepReason(result, "TychoPublisher.Model", IncrementalStepRunReason.Modified);
        AssertStepReason(result, "TychoEventSerializer.Model", IncrementalStepRunReason.Modified);
        AssertStepReason(result, "TychoSetup.Model", IncrementalStepRunReason.Modified);
        AssertStepReason(result, "TychoAppBuilder.Model", IncrementalStepRunReason.Modified);
        AssertStepReason(result, "TychoExtensions.Model", IncrementalStepRunReason.Modified);
    }

    [Fact]
    public void UnrelatedEdits_DoNotInvalidateAnySteps()
    {
        GeneratorDriverRunResult result = RunTrackedEdit(
            "OrderCreatedEvent.cs",
            "public class OrderCreatedEvent : IEvent { }",
            "public class OrderCreatedEvent : IEvent { /* unrelated edit */ }");

        AssertStepReason(result, "TychoAppBuilder.Model", IncrementalStepRunReason.Cached);
        AssertStepReason(result, "TychoExtensions.Model", IncrementalStepRunReason.Cached);
        AssertStepReason(result, "TychoFacade.Model", IncrementalStepRunReason.Cached);
        AssertStepReason(result, "TychoPublisher.Model", IncrementalStepRunReason.Cached);
        AssertStepReason(result, "TychoEventSerializer.Model", IncrementalStepRunReason.Cached);
        AssertStepReason(result, "TychoSetup.Model", IncrementalStepRunReason.Cached);
    }

    private static void AssertStepReason(
        GeneratorDriverRunResult result,
        string trackingName,
        IncrementalStepRunReason expectedReason)
    {
        IncrementalGeneratorRunStep[] steps = [.. result.Results.Single().TrackedSteps[trackingName]];
        Assert.NotEmpty(steps);
        Assert.All(steps.SelectMany(step => step.Outputs), output => Assert.Equal(expectedReason, output.Reason));
    }

    private static GeneratorDriverRunResult RunTrackedEdit(
        string targetFileName,
        string oldText,
        string newText)
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        string[] sources =
        [
            "AppWithEvents/TestApp.cs",
            "AppWithEvents/Events/OrderCreatedEvent.cs",
            "AppWithEvents/Events/PaymentFailedEvent.cs",
            "AppWithEvents/Events/PaymentProcessedEvent.cs",
            "AppWithEvents/Handlers/OrderCreatedEventHandler.cs",
            "AppWithEvents/Handlers/PaymentProcessedEventHandler.cs",
            "AppWithEvents/Modules/ModuleA.cs"
        ];

        CSharpCompilation compilation = CreateCompilation("IncrementalTests\\SUT", sources);
        GeneratorDriver driver = CreateGeneratorDriverWithTracking();
        driver = driver.RunGenerators(compilation, cancellationToken);

        SyntaxTree syntaxTree = compilation.SyntaxTrees.Single(tree => tree.FilePath.EndsWith(targetFileName, StringComparison.Ordinal));
        string originalSource = syntaxTree.GetText(cancellationToken).ToString();
        string updatedSource = originalSource.Replace(oldText, newText, StringComparison.Ordinal);
        Assert.NotEqual(originalSource, updatedSource);

        SyntaxTree updatedSyntaxTree = CSharpSyntaxTree.ParseText(
            updatedSource,
            path: syntaxTree.FilePath,
            cancellationToken: cancellationToken);
        compilation = compilation.ReplaceSyntaxTree(syntaxTree, updatedSyntaxTree);

        driver = driver.RunGenerators(compilation, cancellationToken);
        return driver.GetRunResult();
    }
}
