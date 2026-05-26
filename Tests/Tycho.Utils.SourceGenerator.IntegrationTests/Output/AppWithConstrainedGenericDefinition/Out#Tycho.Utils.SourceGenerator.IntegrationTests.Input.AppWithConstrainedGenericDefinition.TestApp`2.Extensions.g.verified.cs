//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition.TestApp`2.Extensions.g.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
