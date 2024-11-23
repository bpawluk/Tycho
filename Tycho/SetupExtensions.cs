using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tycho.Apps;

namespace Tycho
{
    public static class SetupExtensions
    {
        public static Task<IHostApplicationBuilder> AddTycho<TApp>(this IHostApplicationBuilder builder)
            where TApp : TychoApp, new()
        {
            return builder.AddTycho(new TApp());
        }

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