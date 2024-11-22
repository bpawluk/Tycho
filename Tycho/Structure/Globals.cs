using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Tycho.Structure
{
    internal class Globals
    {
        public IConfiguration Configuration { get; set; }

        public Action<ILoggingBuilder>? LoggingSetup { get; set; }

        public Globals()
        {
            Configuration = new ConfigurationBuilder().Build();
        }
    }
}