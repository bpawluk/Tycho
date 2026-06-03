//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition.TestApp`2.Extensions.g.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tycho.Logging;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.SharedConstraints;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition
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
        public static TestApp<TPayload, TKey> WithConfiguration<TPayload, TKey>(this TestApp<TPayload, TKey> app, IConfiguration globalConfiguration)
            where TPayload : PayloadBase, IMarker, new()
            where TKey : notnull
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
        public static TestApp<TPayload, TKey> WithLogging<TPayload, TKey>(this TestApp<TPayload, TKey> app, Action<ILoggingBuilder> loggingSetup)
            where TPayload : PayloadBase, IMarker, new()
            where TKey : notnull
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
        public static async Task<ITestApp<TPayload, TKey>> RunAsync<TPayload, TKey>(this TestApp<TPayload, TKey> app)
            where TPayload : PayloadBase, IMarker, new()
            where TKey : notnull
        {
            var appInstance = await app.RunBaseAsync().ConfigureAwait(false);
            return new TestAppFacade<TPayload, TKey>(appInstance);
        }

        /// <summary>
        /// Sets up and runs the specified Tycho application and registers it in the host application builder.
        /// </summary>
        /// <param name="builder">The host application builder to extend.</param>
        /// <param name="app">An instance of the application to run.</param>
        /// <returns>The host application builder.</returns>
        public static async Task<IHostApplicationBuilder> AddTestApp<TPayload, TKey>(this IHostApplicationBuilder builder, TestApp<TPayload, TKey> app)
            where TPayload : PayloadBase, IMarker, new()
            where TKey : notnull
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
