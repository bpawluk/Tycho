using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tycho.Utils;

namespace Tycho.Logging
{
    /// <summary>
    /// Provides methods to configure logging for Tycho applications.
    /// </summary>
    [ReferencedBySourceGenerator]
    public static class LoggingConfiguration
    {
        /// <summary>
        /// Configures the logging builder with settings from the configuration and sets up default console logging.
        /// </summary>
        /// <param name="logging">The logging builder to configure.</param>
        /// <param name="configuration">The configuration instance containing logging settings.</param>
        [ReferencedBySourceGenerator]
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
