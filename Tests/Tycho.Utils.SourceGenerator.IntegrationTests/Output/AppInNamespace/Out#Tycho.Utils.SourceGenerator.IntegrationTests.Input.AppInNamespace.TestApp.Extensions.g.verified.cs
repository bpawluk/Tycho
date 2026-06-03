//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInNamespace.TestApp.Extensions.g.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tycho.Logging;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInNamespace
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
        public static TestApp WithConfiguration(this TestApp app, IConfiguration globalConfiguration)
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
        public static TestApp WithLogging(this TestApp app, Action<ILoggingBuilder> loggingSetup)
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
        public static async Task<ITestApp> RunAsync(this TestApp app)
        {
            var appInstance = await app.RunBaseAsync().ConfigureAwait(false);
            return new TestAppFacade(appInstance);
        }

        /// <summary>
        /// Sets up and runs the specified Tycho application and registers it in the host application builder.
        /// </summary>
        /// <param name="builder">The host application builder to extend.</param>
        /// <param name="app">An instance of the application to run.</param>
        /// <returns>The host application builder.</returns>
        public static async Task<IHostApplicationBuilder> AddTestApp(this IHostApplicationBuilder builder, TestApp app)
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
