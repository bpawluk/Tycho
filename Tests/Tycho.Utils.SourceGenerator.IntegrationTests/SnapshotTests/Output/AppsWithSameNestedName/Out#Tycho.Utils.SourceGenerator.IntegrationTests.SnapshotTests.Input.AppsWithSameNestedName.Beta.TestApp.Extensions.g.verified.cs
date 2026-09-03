//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppsWithSameNestedName.Beta.TestApp.Extensions.g.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Tycho.Hosting.Services;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppsWithSameNestedName
{
    public static partial class TestAppSetupExtensions
    {
        public static Beta.TestAppBuilder CreateAppBuilder(this Beta.TestApp app)
        {
            var appBuilderBase = app.CreateAppBuilderBase();
            return new Beta.TestAppBuilder(appBuilderBase);
        }

        public static IHostApplicationBuilder AddTestApp(this IHostApplicationBuilder builder, Beta.TestApp appDefinition)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (appDefinition == null)
            {
                throw new ArgumentNullException(nameof(appDefinition));
            }

            if (Enumerable.Any(builder.Services, descriptor => descriptor.ServiceType == typeof(Beta.ITestApp)))
            {
                throw new InvalidOperationException("The application is already registered in the host.");
            }

            Beta.TestAppBuilder appBuilder = appDefinition.CreateAppBuilder();
            ServiceCollectionServiceExtensions.AddSingleton(builder.Services, provider => appBuilder.Build(provider));
            ServiceCollectionHostedServiceExtensions.AddHostedService<AppHostedLifecycleService<Beta.ITestApp>>(builder.Services);

            return builder;
        }
    }
}
