//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppsWithSameNestedName.Alpha.TestApp.Extensions.g.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Tycho.Hosting.Services;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppsWithSameNestedName
{
    public static partial class TestAppSetupExtensions
    {
        public static Alpha.TestAppBuilder CreateAppBuilder(this Alpha.TestApp app)
        {
            var appBuilderBase = app.CreateAppBuilderBase();
            return new Alpha.TestAppBuilder(appBuilderBase);
        }

        public static IHostApplicationBuilder AddTestApp(this IHostApplicationBuilder builder, Alpha.TestApp appDefinition)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (appDefinition == null)
            {
                throw new ArgumentNullException(nameof(appDefinition));
            }

            if (Enumerable.Any(builder.Services, descriptor => descriptor.ServiceType == typeof(Alpha.ITestApp)))
            {
                throw new InvalidOperationException("The application is already registered in the host.");
            }

            Alpha.TestAppBuilder appBuilder = appDefinition.CreateAppBuilder();
            ServiceCollectionServiceExtensions.AddSingleton(builder.Services, provider => appBuilder.Build(provider));
            ServiceCollectionHostedServiceExtensions.AddHostedService<AppHostedLifecycleService<Alpha.ITestApp>>(builder.Services);

            return builder;
        }
    }
}
