//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithEvents.TestApp.Setup.g.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tycho.Apps;
using Tycho.Events.Serialization;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithEvents
{
    public partial class TestApp : TychoApp
    {
        /// <summary>
        /// Supplies global configuration for the application and its modules.
        /// </summary>
        /// <param name="globalConfiguration">Configuration to be used</param>
        /// <returns>The current <see cref="TestApp"/> instance.</returns>
        /// <exception cref="ArgumentNullException"/>
        public TestApp WithConfiguration(IConfiguration globalConfiguration)
        {
            WithConfigurationBase(globalConfiguration);
            return this;
        }

        /// <summary>
        /// Supplies logging setup for the application and its modules.
        /// </summary>
        /// <param name="loggingSetup">Logging setup to be used</param>
        /// <returns>The current <see cref="TestApp"/> instance.</returns>
        /// <exception cref="ArgumentNullException"/>
        public TestApp WithLogging(Action<ILoggingBuilder> loggingSetup)
        {
            WithLoggingBase(loggingSetup);
            return this;
        }

        /// <summary>
        /// Builds and runs the application according to the definition.
        /// </summary>
        /// <returns>A fresh and ready to use instance of the application</returns>
        /// <exception cref="InvalidOperationException"/>
        public async Task<ITestApp> RunAsync()
        {
            var appInstance = await RunBaseAsync().ConfigureAwait(false);
            return new TestAppFacade(appInstance);
        }

        protected override void __AutoSetup__(IServiceCollection app)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestAppEventSerializer>(app);
            ServiceCollectionServiceExtensions.AddTransient<ITestAppPublisher, TestAppPublisher>(app);
        }
    }
}
