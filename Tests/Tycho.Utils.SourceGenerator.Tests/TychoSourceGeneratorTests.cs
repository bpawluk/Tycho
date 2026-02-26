using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;

namespace Tycho.Utils.SourceGenerator.Tests;

public class TychoSourceGeneratorTests
{
    [Fact]
    public void TychoSourceGenerator_GeneratesAllExpectedFiles()
    {
        Compilation inputCompilation = CreateCompilation(@"
            using Microsoft.Extensions.DependencyInjection;
            using SampleApp.AppEventHandlers;
            using SampleApp.AppEvents;
            using SampleApp.ModuleEventHandlers;
            using SampleApp.ModuleEvents;
            using Tycho;
            using Tycho.Apps;
            using Tycho.Events;
            using Tycho.Modules;

            namespace SampleApp.App
            {
                public class Outer 
                {
                    public class Inner 
                    {
                        [TychoDefinition]
                        public partial class TestApp : TychoApp
                        {
                            protected override void DefineContract(IAppContract app) { }

                            protected override void DefineEvents(IAppEvents app)
                            {
                                app.Handles<TestAppEvent, TestAppEventHandler>();
                                app.Handles<OtherTestAppEvent, OtherTestAppEventHandler>();
                            }

                            protected override void IncludeModules(IAppStructure app) { }

                            protected override void RegisterServices(IServiceCollection app) { }
                        }
                    }
                    
                }
            }

            [TychoDefinition]
            public partial class TestModule : TychoModule
            {
                protected override void DefineContract(IModuleContract module) { }

                protected override void DefineEvents(IModuleEvents module)
                {
                    module.Handles<TestModuleEvent, TestModuleEventHandler>();
                    module.Handles<OtherTestModuleEvent, OtherTestModuleEventHandler>();
                }

                protected override void IncludeModules(IModuleStructure module) { }

                protected override void RegisterServices(IServiceCollection module) { }
            }

            namespace SampleApp.AppEvents
            {
                public class TestAppEvent : IEvent { }
                public class OtherTestAppEvent : IEvent { }
            }

            namespace SampleApp.AppEventHandlers
            {
                public class TestAppEventHandler : IEventHandler<TestAppEvent>
                {
                    public Task Handle(EventContext<TestAppEvent> context, CancellationToken cancellationToken)
                    {
                        throw new NotImplementedException();
                    }
                }

                public class OtherTestAppEventHandler : IEventHandler<OtherTestAppEvent>
                {
                    public Task Handle(EventContext<OtherTestAppEvent> context, CancellationToken cancellationToken)
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            namespace SampleApp.ModuleEvents
            {
                public class TestModuleEvent : IEvent { }
                public class OtherTestModuleEvent : IEvent { }
            }

            namespace SampleApp.ModuleEventHandlers
            {
                public class TestModuleEventHandler : IEventHandler<TestModuleEvent>
                {
                    public Task Handle(EventContext<TestModuleEvent> context, CancellationToken cancellationToken)
                    {
                        throw new NotImplementedException();
                    }
                }

                public class OtherTestModuleEventHandler : IEventHandler<OtherTestModuleEvent>
                {
                    public Task Handle(EventContext<OtherTestModuleEvent> context, CancellationToken cancellationToken)
                    {
                        throw new NotImplementedException();
                    }
                }
            }");

        var generator = new TychoSourceGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

        var runResult = driver.GetRunResult();
        Assert.Empty(runResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Single(runResult.Results);

        var generatedTrees = outputCompilation.SyntaxTrees.Except(inputCompilation.SyntaxTrees).ToArray();
        Assert.NotEmpty(generatedTrees);

        var generatedFileNames = generatedTrees
            .Select(t => Path.GetFileName(t.FilePath))
            .ToArray();

        Assert.Contains("SampleApp.App.Outer.Inner.TestApp.g.cs", generatedFileNames);
        Assert.Contains("SampleApp.App.Outer.Inner.TestApp.Setup.g.cs", generatedFileNames);
        Assert.Contains("SampleApp.App.Outer.Inner.TestApp.Facade.g.cs", generatedFileNames);
        Assert.Contains("SampleApp.App.Outer.Inner.TestApp.Facade.Interface.g.cs", generatedFileNames);
        Assert.Contains("SampleApp.App.Outer.Inner.TestApp.Events.Dispatcher.g.cs", generatedFileNames);
        Assert.Contains("TestModule.g.cs", generatedFileNames);
        Assert.Contains("TestModule.Events.Dispatcher.g.cs", generatedFileNames);
    }

    private static CSharpCompilation CreateCompilation(string source)
        => CSharpCompilation.Create("compilation",
            [CSharpSyntaxTree.ParseText(source)],
            [
                MetadataReference.CreateFromFile(typeof(TychoDefinitionAttribute).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IServiceCollection).GetTypeInfo().Assembly.Location)
            ],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
}
