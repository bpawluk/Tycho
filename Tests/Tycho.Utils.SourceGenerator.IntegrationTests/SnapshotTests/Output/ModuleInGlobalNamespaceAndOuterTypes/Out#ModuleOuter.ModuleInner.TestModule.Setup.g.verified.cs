//HintName: ModuleOuter.ModuleInner.TestModule.Setup.g.cs
public partial class ModuleOuter
{
    public partial class ModuleInner
    {
        public class TestModuleSetup
        {
            public static void Setup(global::Microsoft.Extensions.DependencyInjection.IServiceCollection module)
            {
                global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton<global::Tycho.Events.Serialization.IEventSerializer, TestModuleEventSerializer>(module);
                global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<ITestModulePublisher, TestModulePublisher>(module);
                global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddTransient<ITestModuleParent, TestModuleParent>(module);
            }
        }
    }
}
