//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInGenericOuterTypes.Outer`1.Inner`1.TestApp`1.Extensions.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInGenericOuterTypes
{
    public static partial class TestAppSetupExtensions
    {
        public static global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInGenericOuterTypes.Outer<TOuter>.Inner<TInner>.TestAppBuilder<TApp> CreateAppBuilder<TOuter, TInner, TApp>(this Outer<TOuter>.Inner<TInner>.TestApp<TApp> app)
            where TOuter : class
            where TInner : notnull
            where TApp : new()
        {
            var appBuilderBase = app.CreateAppBuilderBase();
            return new global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInGenericOuterTypes.Outer<TOuter>.Inner<TInner>.TestAppBuilder<TApp>(appBuilderBase);
        }

        public static global::Microsoft.Extensions.Hosting.IHostApplicationBuilder AddTestApp<TOuter, TInner, TApp>(this global::Microsoft.Extensions.Hosting.IHostApplicationBuilder builder, Outer<TOuter>.Inner<TInner>.TestApp<TApp> appDefinition)
            where TOuter : class
            where TInner : notnull
            where TApp : new()
        {
            if (builder == null)
            {
                throw new global::System.ArgumentNullException(nameof(builder));
            }

            if (appDefinition == null)
            {
                throw new global::System.ArgumentNullException(nameof(appDefinition));
            }

            if (global::System.Linq.Enumerable.Any(builder.Services, descriptor => descriptor.ServiceType == typeof(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInGenericOuterTypes.Outer<TOuter>.Inner<TInner>.ITestApp<TApp>)))
            {
                throw new global::System.InvalidOperationException("The application is already registered in the host.");
            }

            global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInGenericOuterTypes.Outer<TOuter>.Inner<TInner>.TestAppBuilder<TApp> appBuilder = appDefinition.CreateAppBuilder();
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton(builder.Services, provider => appBuilder.Build(provider));
            global::Microsoft.Extensions.DependencyInjection.ServiceCollectionHostedServiceExtensions.AddHostedService<global::Tycho.Hosting.Services.AppHostedLifecycleService<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInGenericOuterTypes.Outer<TOuter>.Inner<TInner>.ITestApp<TApp>>>(builder.Services);

            return builder;
        }
    }
}
