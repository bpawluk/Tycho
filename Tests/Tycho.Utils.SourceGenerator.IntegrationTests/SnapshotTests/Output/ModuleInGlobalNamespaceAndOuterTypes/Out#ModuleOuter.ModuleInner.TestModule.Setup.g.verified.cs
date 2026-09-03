//HintName: ModuleOuter.ModuleInner.TestModule.Setup.g.cs
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Serialization;

public partial class ModuleOuter
{
    public partial class ModuleInner
    {
        public class TestModuleSetup
        {
            public static void Setup(IServiceCollection module)
            {
                ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestModuleEventSerializer>(module);
                ServiceCollectionServiceExtensions.AddTransient<ITestModulePublisher, TestModulePublisher>(module);
                ServiceCollectionServiceExtensions.AddTransient<ITestModuleParent, TestModuleParent>(module);
            }
        }
    }
}
