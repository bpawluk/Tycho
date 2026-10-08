//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.TestApp`2.Extensions.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition
{
    public static partial class TestAppSetupExtensions
    {
        public static global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.TestAppBuilder<TPayload, TKey> CreateAppBuilder<TPayload, TKey>(this TestApp<TPayload, TKey> app)
            where TPayload : global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.Model.PayloadBase, global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.IMarker, new()
            where TKey : notnull
        {
            var appBuilderBase = app.CreateAppBuilderBase();
            return new global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.TestAppBuilder<TPayload, TKey>(appBuilderBase);
        }

        public static global::Microsoft.Extensions.Hosting.IHostApplicationBuilder AddTestApp<TPayload, TKey>(this global::Microsoft.Extensions.Hosting.IHostApplicationBuilder builder, TestApp<TPayload, TKey> appDefinition)
            where TPayload : global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.Model.PayloadBase, global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.IMarker, new()
            where TKey : notnull
        {
            if (builder == null)
            {
                throw new global::System.ArgumentNullException(nameof(builder));
            }

            if (appDefinition == null)
            {
                throw new global::System.ArgumentNullException(nameof(appDefinition));
            }

            if (global::System.Linq.Enumerable.Any(builder.Services, descriptor => descriptor.ServiceType == typeof(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.ITestApp<TPayload, TKey>)))
            {
                throw new global::System.InvalidOperationException("The application is already registered in the host.");
            }

            global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.TestAppBuilder<TPayload, TKey> appBuilder = appDefinition.CreateAppBuilder();
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton(builder.Services, provider => appBuilder.Build(provider));
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionHostedServiceExtensions.AddHostedService<global::Tycho.Hosting.Services.AppHostedLifecycleService<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.ITestApp<TPayload, TKey>>>(builder.Services);

            return builder;
        }
    }
}
