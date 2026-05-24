//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInGenericOuterTypes.Outer`1.Inner`1.TestApp`1.Extensions.g.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
