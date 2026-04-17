using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;

namespace Tycho.Utils.SourceGenerator.IntegrationTests;

public class TychoSourceGeneratorTests : VerifyBase
{
    public TychoSourceGeneratorTests() : base() { }

    [Fact]
    public Task AppInGlobalNamespace()
    {
        string[] sources =
        [
            "AppInGlobalNamespace/TestApp.cs"
        ];
        var driver = RunGenerator(sources);
        return Verify(driver);
    }

    [Fact]
    public Task AppInGlobalNamespaceAndOuterTypes()
    {
        string[] sources =
        [
            "AppInGlobalNamespaceAndOuterTypes/TestApp.cs"
        ];
        var driver = RunGenerator(sources);
        return Verify(driver);
    }

    [Fact]
    public Task AppInNamespace()
    {
        string[] sources =
        [
            "AppInNamespace/TestApp.cs"
        ];
        var driver = RunGenerator(sources);
        return Verify(driver);
    }

    [Fact]
    public Task AppInNamespaceAndOuterTypes()
    {
        string[] sources =
        [
            "AppInNamespaceAndOuterTypes/TestApp.cs"
        ];
        var driver = RunGenerator(sources);
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
        var driver = RunGenerator(sources);
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
        var driver = RunGenerator(sources);
        return Verify(driver);
    }

    [Fact]
    public Task AppWithoutAttribute()
    {
        string[] sources =
        [
            "AppWithoutAttribute/TestApp.cs"
        ];
        var driver = RunGenerator(sources);
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
        var driver = RunGenerator(sources);
        return Verify(driver);
    }

    [Fact]
    public Task ModuleInGlobalNamespace()
    {
        string[] sources =
        [
            "ModuleInGlobalNamespace/TestModule.cs"
        ];
        var driver = RunGenerator(sources);
        return Verify(driver);
    }

    [Fact]
    public Task ModuleInGlobalNamespaceAndOuterTypes()
    {
        string[] sources =
        [
            "ModuleInGlobalNamespaceAndOuterTypes/TestModule.cs"
        ];
        var driver = RunGenerator(sources);
        return Verify(driver);
    }

    [Fact]
    public Task ModuleInNamespace()
    {
        string[] sources =
        [
            "ModuleInNamespace/TestModule.cs"
        ];
        var driver = RunGenerator(sources);
        return Verify(driver);
    }

    [Fact]
    public Task ModuleInNamespaceAndOuterTypes()
    {
        string[] sources =
        [
            "ModuleInNamespaceAndOuterTypes/TestModule.cs"
        ];
        var driver = RunGenerator(sources);
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
        var driver = RunGenerator(sources);
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
        var driver = RunGenerator(sources);
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
        var driver = RunGenerator(sources);
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
        var driver = RunGenerator(sources);
        return Verify(driver);
    }

    [Fact]
    public Task NonClassTypes()
    {
        string[] sources =
        [
            "NonClassTypes/TestInterface.cs",
            "NonClassTypes/TestStruct.cs"
        ];
        var driver = RunGenerator(sources);
        return Verify(driver);
    }

    private static GeneratorDriver RunGenerator(string[] sources)
    {
        var compilation = CreateCompilation(sources);
        var generator = new TychoSourceGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        return driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out _);
    }

    private static CSharpCompilation CreateCompilation(string[] sources)
    {
        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        var syntaxTrees = sources.Select(source =>
        {
            var sourcePath = Path.Combine(AppContext.BaseDirectory, "Input", source);
            var sourceContent = File.ReadAllText(sourcePath);
            return CSharpSyntaxTree.ParseText(sourceContent);
        });
        PortableExecutableReference[] references =
        [
            MetadataReference.CreateFromFile(typeof(TychoDefinitionAttribute).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IServiceCollection).GetTypeInfo().Assembly.Location)
        ];
        return CSharpCompilation.Create("Compilation", syntaxTrees, references, options);
    }
}
