//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition.TestApp`2.Setup.g.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tycho.Apps;
using Tycho.Events.Serialization;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition
{
    public partial class TestApp<TPayload, TKey> : TychoApp
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
        /// <summary>
        /// Supplies global configuration for the application and its modules.
        /// </summary>
        /// <param name="globalConfiguration">Configuration to be used</param>
        /// <returns>The current <see cref="TestApp<TPayload, TKey>"/> instance.</returns>
        /// <exception cref="ArgumentNullException"/>
        public TestApp<TPayload, TKey> WithConfiguration(IConfiguration globalConfiguration)
        {
            WithConfigurationBase(globalConfiguration);
            return this;
        }

        /// <summary>
        /// Supplies logging setup for the application and its modules.
        /// </summary>
        /// <param name="loggingSetup">Logging setup to be used</param>
        /// <returns>The current <see cref="TestApp<TPayload, TKey>"/> instance.</returns>
        /// <exception cref="ArgumentNullException"/>
        public TestApp<TPayload, TKey> WithLogging(Action<ILoggingBuilder> loggingSetup)
        {
            WithLoggingBase(loggingSetup);
            return this;
        }

        /// <summary>
        /// Builds and runs the application according to the definition.
        /// </summary>
        /// <returns>A fresh and ready to use instance of the application</returns>
        /// <exception cref="InvalidOperationException"/>
        public async Task<ITestApp<TPayload, TKey>> RunAsync()
        {
            var appInstance = await RunBaseAsync().ConfigureAwait(false);
            return new TestAppFacade<TPayload, TKey>(appInstance);
        }

        protected override void __AutoSetup__(IServiceCollection app)
        {
            ServiceCollectionServiceExtensions.AddSingleton<IEventSerializer, TestAppEventSerializer<TPayload, TKey>>(app);
            ServiceCollectionServiceExtensions.AddTransient<IPublisher, TestAppPublisher<TPayload, TKey>>(app);
        }
    }
}
