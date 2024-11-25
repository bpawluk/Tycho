using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tycho.Apps;

namespace Tycho
{
    /// <summary>
    /// Extension methods for setting up Tycho applications
    /// </summary>
    public static class SetupExtensions
    {
        /// <summary>
        /// Sets up and runs the specified Tycho application and registers it in the Host Application Builder
        /// </summary>
        /// <typeparam name="TApp">The type of the application to run</typeparam>
        public static Task<IHostApplicationBuilder> AddTycho<TApp>(this IHostApplicationBuilder builder)
            where TApp : TychoApp, new()
        {
            return builder.AddTycho(new TApp());
        }

        /// <summary>
        /// Sets up and runs the specified Tycho application and registers it in the Host Application Builder
        /// </summary>
        /// <typeparam name="TApp">The type of the application to run</typeparam>
        /// <param name="app">An instance of the application to run</param>
        public static async Task<IHostApplicationBuilder> AddTycho<TApp>(this IHostApplicationBuilder builder, TApp app)
            where TApp : TychoApp
        {
            var appInstance = await app
                .WithConfiguration(builder.Configuration)
                .WithLogging(logging =>
                {
                    logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.Configure(options =>
                    {
                        options.ActivityTrackingOptions =
                            ActivityTrackingOptions.SpanId |
                            ActivityTrackingOptions.TraceId |
                            ActivityTrackingOptions.ParentId;
                    });
                })
                .Run()
                .ConfigureAwait(false);

            builder.Services.AddSingleton(appInstance);

            return builder;
        }
    }
}