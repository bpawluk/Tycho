//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition.TestApp`2.Extensions.g.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Tycho.Hosting.Services;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.SharedConstraints;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition
{
    public static partial class TestAppSetupExtensions
    {
        public static TestAppBuilder<TPayload, TKey> CreateAppBuilder<TPayload, TKey>(this TestApp<TPayload, TKey> app)
            where TPayload : PayloadBase, IMarker, new()
            where TKey : notnull
        {
            var appBuilderBase = app.CreateAppBuilderBase();
            return new TestAppBuilder<TPayload, TKey>(appBuilderBase);
        }

        public static IHostApplicationBuilder AddTestApp<TPayload, TKey>(this IHostApplicationBuilder builder, TestApp<TPayload, TKey> appDefinition)
            where TPayload : PayloadBase, IMarker, new()
            where TKey : notnull
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (appDefinition == null)
            {
                throw new ArgumentNullException(nameof(appDefinition));
            }

            if (Enumerable.Any(builder.Services, descriptor => descriptor.ServiceType == typeof(ITestApp<TPayload, TKey>)))
            {
                throw new InvalidOperationException("The application is already registered in the host.");
            }

            TestAppBuilder<TPayload, TKey> appBuilder = appDefinition.CreateAppBuilder();
            ServiceCollectionServiceExtensions.AddSingleton(builder.Services, provider => appBuilder.Build(provider));
            ServiceCollectionHostedServiceExtensions.AddHostedService<AppHostedLifecycleService<ITestApp<TPayload, TKey>>>(builder.Services);

            return builder;
        }
    }
}
