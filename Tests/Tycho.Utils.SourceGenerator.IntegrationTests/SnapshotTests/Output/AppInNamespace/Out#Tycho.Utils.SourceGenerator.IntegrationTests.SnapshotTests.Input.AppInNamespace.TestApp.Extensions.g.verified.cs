//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInNamespace.TestApp.Extensions.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInNamespace
{
    public static partial class TestAppSetupExtensions
    {
        public static global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInNamespace.TestAppBuilder CreateAppBuilder(this TestApp app)
        {
            var appBuilderBase = app.CreateAppBuilderBase();
            return new global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInNamespace.TestAppBuilder(appBuilderBase);
        }

        public static global::Microsoft.Extensions.Hosting.IHostApplicationBuilder AddTestApp(this global::Microsoft.Extensions.Hosting.IHostApplicationBuilder builder, TestApp appDefinition)
        {
            if (builder == null)
            {
                throw new global::System.ArgumentNullException(nameof(builder));
            }

            if (appDefinition == null)
            {
                throw new global::System.ArgumentNullException(nameof(appDefinition));
            }

            if (global::System.Linq.Enumerable.Any(builder.Services, descriptor => descriptor.ServiceType == typeof(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInNamespace.ITestApp)))
            {
                throw new global::System.InvalidOperationException("The application is already registered in the host.");
            }

            global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInNamespace.TestAppBuilder appBuilder = appDefinition.CreateAppBuilder();
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton(builder.Services, provider => appBuilder.Build(provider));
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionHostedServiceExtensions.AddHostedService<global::Tycho.Hosting.Services.AppHostedLifecycleService<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInNamespace.ITestApp>>(builder.Services);

            return builder;
        }
    }
}
