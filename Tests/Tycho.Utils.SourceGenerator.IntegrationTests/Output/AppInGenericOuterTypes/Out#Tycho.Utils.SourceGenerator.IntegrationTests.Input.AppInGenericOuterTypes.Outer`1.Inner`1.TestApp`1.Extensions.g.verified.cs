//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInGenericOuterTypes.Outer`1.Inner`1.TestApp`1.Extensions.g.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Tycho.Hosting.Services;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInGenericOuterTypes
{
    public static partial class TestAppSetupExtensions
    {
        public static TestAppBuilder<TOuter, TInner, TApp> CreateAppBuilder<TOuter, TInner, TApp>(this Outer<TOuter>.Inner<TInner>.TestApp<TApp> app)
            where TOuter : class
            where TInner : notnull
            where TApp : new()
        {
            var appBuilderBase = app.CreateAppBuilderBase();
            return new TestAppBuilder<TOuter, TInner, TApp>(appBuilderBase);
        }

        public static IHostApplicationBuilder AddTestApp<TOuter, TInner, TApp>(this IHostApplicationBuilder builder, Outer<TOuter>.Inner<TInner>.TestApp<TApp> appDefinition)
            where TOuter : class
            where TInner : notnull
            where TApp : new()
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (appDefinition == null)
            {
                throw new ArgumentNullException(nameof(appDefinition));
            }

            if (Enumerable.Any(builder.Services, descriptor => descriptor.ServiceType == typeof(ITestApp<TApp>)))
            {
                throw new InvalidOperationException("The application is already registered in the host.");
            }

            TestAppBuilder<TOuter, TInner, TApp> appBuilder = appDefinition.CreateAppBuilder();
            ServiceCollectionServiceExtensions.AddSingleton(builder.Services, provider => appBuilder.Build(provider));
            ServiceCollectionHostedServiceExtensions.AddHostedService<AppHostedLifecycleService<ITestApp<TApp>>>(builder.Services);

            return builder;
        }
    }
}
