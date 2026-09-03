//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithGenericDefinition.TestApp`1.Extensions.g.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Tycho.Hosting.Services;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithGenericDefinition
{
    public static partial class TestAppSetupExtensions
    {
        public static TestAppBuilder<T> CreateAppBuilder<T>(this TestApp<T> app)
        {
            var appBuilderBase = app.CreateAppBuilderBase();
            return new TestAppBuilder<T>(appBuilderBase);
        }

        public static IHostApplicationBuilder AddTestApp<T>(this IHostApplicationBuilder builder, TestApp<T> appDefinition)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (appDefinition == null)
            {
                throw new ArgumentNullException(nameof(appDefinition));
            }

            if (Enumerable.Any(builder.Services, descriptor => descriptor.ServiceType == typeof(ITestApp<T>)))
            {
                throw new InvalidOperationException("The application is already registered in the host.");
            }

            TestAppBuilder<T> appBuilder = appDefinition.CreateAppBuilder();
            ServiceCollectionServiceExtensions.AddSingleton(builder.Services, provider => appBuilder.Build(provider));
            ServiceCollectionHostedServiceExtensions.AddHostedService<AppHostedLifecycleService<ITestApp<T>>>(builder.Services);

            return builder;
        }
    }
}
