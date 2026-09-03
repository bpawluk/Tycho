using Microsoft.CodeAnalysis;
using static Tycho.Utils.SourceGenerator.IntegrationTests._Utils.RunHelpers;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests;

public class SnapshotTests : VerifyBase
{
    private const string SourceDir = "SnapshotTests\\Input";

    public SnapshotTests() : base() { }

    [Fact]
    public Task AppInGlobalNamespace()
    {
        string[] sources =
        [
            "AppInGlobalNamespace/TestApp.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task AppInGlobalNamespaceAndOuterTypes()
    {
        string[] sources =
        [
            "AppInGlobalNamespaceAndOuterTypes/TestApp.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task AppInNamespace()
    {
        string[] sources =
        [
            "AppInNamespace/TestApp.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task AppInNamespaceAndOuterTypes()
    {
        string[] sources =
        [
            "AppInNamespaceAndOuterTypes/TestApp.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task AppInGenericOuterTypes()
    {
        string[] sources =
        [
            "AppInGenericOuterTypes/TestApp.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task AppsWithSameNestedName()
    {
        string[] sources =
        [
            "AppsWithSameNestedName/AlphaTestApp.cs",
            "AppsWithSameNestedName/BetaTestApp.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task AppWithGenericDefinition()
    {
        string[] sources =
        [
            "AppWithGenericDefinition/TestApp.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task AppWithConstrainedGenericDefinition()
    {
        string[] sources =
        [
            "AppWithConstrainedGenericDefinition/TestApp.cs",
            "AppWithConstrainedGenericDefinition/Model/PayloadBase.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task AppWithDownstreamContract()
    {
        string[] sources =
        [
            "AppWithDownstreamContract/TestApp.cs",
            "AppWithDownstreamContract/Handlers/DeleteItemCommandHandler.cs",
            "AppWithDownstreamContract/Handlers/GetItemQueryHandler.cs",
            "AppWithDownstreamContract/Requests/DeleteItemCommand.cs",
            "AppWithDownstreamContract/Requests/GetItemQuery.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task AppWithEvents()
    {
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
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task AppWithoutAttribute()
    {
        string[] sources =
        [
            "AppWithoutAttribute/TestApp.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task AppWithSubmodules()
    {
        string[] sources =
        [
            "AppWithSubmodules/TestApp.cs",
            "AppWithSubmodules/Modules/ModuleA.cs",
            "AppWithSubmodules/Modules/ModuleB.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task AppWithIndirectDefinitions()
    {
        string[] sources =
        [
            "AppWithIndirectDefinitions/TestApp.cs",
            "AppWithIndirectDefinitions/Helpers/HelperClass.cs",
            "AppWithIndirectDefinitions/Helpers/HelperStaticClass.cs",
            "AppWithIndirectDefinitions/Handlers/TestRequestHandler.cs",
            "AppWithIndirectDefinitions/Handlers/TestEventHandler.cs",
            "AppWithIndirectDefinitions/Modules/LocalHelperModule.cs",
            "AppWithIndirectDefinitions/Modules/LocalStaticHelperModule.cs",
            "AppWithIndirectDefinitions/Modules/HelperClassModule.cs",
            "AppWithIndirectDefinitions/Modules/HelperStaticClassModule.cs",
            "AppWithIndirectDefinitions/Modules/HelperExtensionModule.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task ModuleInGlobalNamespace()
    {
        string[] sources =
        [
            "ModuleInGlobalNamespace/TestModule.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task ModuleInGlobalNamespaceAndOuterTypes()
    {
        string[] sources =
        [
            "ModuleInGlobalNamespaceAndOuterTypes/TestModule.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task ModuleInNamespace()
    {
        string[] sources =
        [
            "ModuleInNamespace/TestModule.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task ModuleInNamespaceAndOuterTypes()
    {
        string[] sources =
        [
            "ModuleInNamespaceAndOuterTypes/TestModule.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task ModuleInGenericOuterTypes()
    {
        string[] sources =
        [
            "ModuleInGenericOuterTypes/TestModule.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task ModuleWithGenericDefinition()
    {
        string[] sources =
        [
            "ModuleWithGenericDefinition/TestModule.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task ModuleWithConstrainedGenericDefinition()
    {
        string[] sources =
        [
            "ModuleWithConstrainedGenericDefinition/TestModule.cs",
            "ModuleWithConstrainedGenericDefinition/Model/PayloadBase.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task ModuleWithDownstreamContract()
    {
        string[] sources =
        [
            "ModuleWithDownstreamContract/TestModule.cs",
            "ModuleWithDownstreamContract/Handlers/DeleteItemCommandHandler.cs",
            "ModuleWithDownstreamContract/Handlers/GetItemQueryHandler.cs",
            "ModuleWithDownstreamContract/Requests/DeleteItemCommand.cs",
            "ModuleWithDownstreamContract/Requests/GetItemQuery.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task ModuleWithEvents()
    {
        string[] sources =
        [
            "ModuleWithEvents/TestModule.cs",
            "ModuleWithEvents/Events/OrderCreatedEvent.cs",
            "ModuleWithEvents/Events/PaymentFailedEvent.cs",
            "ModuleWithEvents/Events/PaymentProcessedEvent.cs",
            "ModuleWithEvents/Handlers/OrderCreatedEventHandler.cs",
            "ModuleWithEvents/Handlers/PaymentProcessedEventHandler.cs",
            "ModuleWithEvents/Modules/ModuleA.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task ModuleWithSubmodules()
    {
        string[] sources =
        [
            "ModuleWithSubmodules/TestModule.cs",
            "ModuleWithSubmodules/Modules/ModuleA.cs",
            "ModuleWithSubmodules/Modules/ModuleB.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }

    [Fact]
    public Task ModuleWithUpstreamContract()
    {
        string[] sources =
        [
            "ModuleWithUpstreamContract/TestModule.cs",
            "ModuleWithUpstreamContract/Requests/GetParentDataQuery.cs",
            "ModuleWithUpstreamContract/Requests/NotifyParentCommand.cs"
        ];
        GeneratorDriver driver = RunGenerator(SourceDir, sources);
        return Verify(driver);
    }
}
