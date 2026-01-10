using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Tycho.Logging
{
    public static class LoggingConfiguration
    {
        public static void ConfigureLogging(ILoggingBuilder logging, IConfiguration configuration)
        {
            logging.AddConfiguration(configuration.GetSection("Logging"));
            logging.AddConsole();
            logging.Configure(options =>
            {
                options.ActivityTrackingOptions =
                    ActivityTrackingOptions.SpanId |
                    ActivityTrackingOptions.TraceId |
                    ActivityTrackingOptions.ParentId;
            });
        }
    }
}
