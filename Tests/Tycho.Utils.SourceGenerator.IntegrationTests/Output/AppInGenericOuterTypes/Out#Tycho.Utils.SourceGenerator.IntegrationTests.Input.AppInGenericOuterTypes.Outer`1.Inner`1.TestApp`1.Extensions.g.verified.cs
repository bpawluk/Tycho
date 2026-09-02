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
        public static Outer<TOuter>.Inner<TInner>.TestAppBuilder<TApp> CreateAppBuilder<TOuter, TInner, TApp>(this Outer<TOuter>.Inner<TInner>.TestApp<TApp> app)
            where TOuter : class
            where TInner : notnull
            where TApp : new()
        {
            var appBuilderBase = app.CreateAppBuilderBase();
            return new Outer<TOuter>.Inner<TInner>.TestAppBuilder<TApp>(appBuilderBase);
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

            if (Enumerable.Any(builder.Services, descriptor => descriptor.ServiceType == typeof(Outer<TOuter>.Inner<TInner>.ITestApp<TApp>)))
            {
                throw new InvalidOperationException("The application is already registered in the host.");
            }

            Outer<TOuter>.Inner<TInner>.TestAppBuilder<TApp> appBuilder = appDefinition.CreateAppBuilder();
            ServiceCollectionServiceExtensions.AddSingleton(builder.Services, provider => appBuilder.Build(provider));
            ServiceCollectionHostedServiceExtensions.AddHostedService<AppHostedLifecycleService<Outer<TOuter>.Inner<TInner>.ITestApp<TApp>>>(builder.Services);

            return builder;
        }
    }
}
