//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInNamespaceAndOuterTypes.Outer.Inner.TestApp.Extensions.g.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Tycho.Hosting.Services;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInNamespaceAndOuterTypes
{
    public static partial class TestAppSetupExtensions
    {
        public static Outer.Inner.TestAppBuilder CreateAppBuilder(this Outer.Inner.TestApp app)
        {
            var appBuilderBase = app.CreateAppBuilderBase();
            return new Outer.Inner.TestAppBuilder(appBuilderBase);
        }

        public static IHostApplicationBuilder AddTestApp(this IHostApplicationBuilder builder, Outer.Inner.TestApp appDefinition)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (appDefinition == null)
            {
                throw new ArgumentNullException(nameof(appDefinition));
            }

            if (Enumerable.Any(builder.Services, descriptor => descriptor.ServiceType == typeof(Outer.Inner.ITestApp)))
            {
                throw new InvalidOperationException("The application is already registered in the host.");
            }

            Outer.Inner.TestAppBuilder appBuilder = appDefinition.CreateAppBuilder();
            ServiceCollectionServiceExtensions.AddSingleton(builder.Services, provider => appBuilder.Build(provider));
            ServiceCollectionHostedServiceExtensions.AddHostedService<AppHostedLifecycleService<Outer.Inner.ITestApp>>(builder.Services);

            return builder;
        }
    }
}
