//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules.TestApp.Extensions.g.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Tycho.Hosting.Services;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules
{
    public static partial class TestAppSetupExtensions
    {
        public static TestAppBuilder CreateAppBuilder(this TestApp app)
        {
            var appBuilderBase = app.CreateAppBuilderBase();
            return new TestAppBuilder(appBuilderBase);
        }

        public static IHostApplicationBuilder AddTestApp(this IHostApplicationBuilder builder, TestApp appDefinition)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (appDefinition == null)
            {
                throw new ArgumentNullException(nameof(appDefinition));
            }

            if (Enumerable.Any(builder.Services, descriptor => descriptor.ServiceType == typeof(ITestApp)))
            {
                throw new InvalidOperationException("The application is already registered in the host.");
            }

            TestAppBuilder appBuilder = appDefinition.CreateAppBuilder();
            ServiceCollectionServiceExtensions.AddSingleton(builder.Services, provider => appBuilder.Build(provider));
            ServiceCollectionHostedServiceExtensions.AddHostedService<AppHostedLifecycleService<ITestApp>>(builder.Services);

            return builder;
        }
    }
}
