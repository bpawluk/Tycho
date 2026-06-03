//HintName: TestModule.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;
using Tycho.Modules;
using Tycho.Modules.Instance;

public partial class TestModule : TychoModule
{
    protected override void __AutoSetup__(IServiceCollection module)
    {
        ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestModuleEventSerializer>(module);
        ServiceCollectionServiceExtensions.AddTransient<ITestModulePublisher, TestModulePublisher>(module);
        ServiceCollectionServiceExtensions.AddTransient<IParent, TestModuleParent>(module);
    }
}
