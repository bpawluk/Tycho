//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInGenericOuterTypes.Outer`1.Inner`1.TestApp`1.Extensions.g.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tycho.Logging;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInGenericOuterTypes
{
    /// <summary>
    /// Extension methods for setting up Tycho applications.
    /// </summary>
    public static partial class TestAppSetupExtensions
    {
        /// <summary>
        /// Supplies global configuration for the application and its modules.
        /// </summary>
        /// <param name="app">An instance of the application to configure.</param>
        /// <param name="globalConfiguration">Configuration to be used</param>
        /// <returns>The current application instance.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Outer<TOuter>.Inner<TInner>.TestApp<TApp> WithConfiguration<TOuter, TInner, TApp>(this Outer<TOuter>.Inner<TInner>.TestApp<TApp> app, IConfiguration globalConfiguration)
            where TOuter : class
            where TInner : notnull
            where TApp : new()
        {
            app.WithConfigurationBase(globalConfiguration);
            return app;
        }

        /// <summary>
        /// Supplies logging setup for the application and its modules.
        /// </summary>
        /// <param name="app">An instance of the application to configure.</param>
        /// <param name="loggingSetup">Logging setup to be used</param>
        /// <returns>The current application instance.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Outer<TOuter>.Inner<TInner>.TestApp<TApp> WithLogging<TOuter, TInner, TApp>(this Outer<TOuter>.Inner<TInner>.TestApp<TApp> app, Action<ILoggingBuilder> loggingSetup)
            where TOuter : class
            where TInner : notnull
            where TApp : new()
        {
            app.WithLoggingBase(loggingSetup);
            return app;
        }

        /// <summary>
        /// Builds and runs the application according to the definition.
        /// </summary>
        /// <param name="app">An instance of the application to run.</param>
        /// <returns>A fresh and ready to use instance of the application facade.</returns>
        /// <exception cref="InvalidOperationException"/>
        public static async Task<ITestApp<TApp>> RunAsync<TOuter, TInner, TApp>(this Outer<TOuter>.Inner<TInner>.TestApp<TApp> app)
            where TOuter : class
            where TInner : notnull
            where TApp : new()
        {
            var appInstance = await app.RunBaseAsync().ConfigureAwait(false);
            return new TestAppFacade<TApp>(appInstance);
        }

        /// <summary>
        /// Sets up and runs the specified Tycho application and registers it in the host application builder.
        /// </summary>
        /// <param name="builder">The host application builder to extend.</param>
        /// <param name="app">An instance of the application to run.</param>
        /// <returns>The host application builder.</returns>
        public static async Task<IHostApplicationBuilder> AddTestApp<TOuter, TInner, TApp>(this IHostApplicationBuilder builder, Outer<TOuter>.Inner<TInner>.TestApp<TApp> app)
            where TOuter : class
            where TInner : notnull
            where TApp : new()
        {
            var appInstance = await app
                .WithConfiguration(builder.Configuration)
                .WithLogging(logging => LoggingConfiguration.ConfigureLogging(logging, builder.Configuration))
                .RunAsync()
                .ConfigureAwait(false);

            ServiceCollectionServiceExtensions.AddSingleton(builder.Services, appInstance);

            return builder;
        }
    }
}
