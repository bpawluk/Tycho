using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Requests;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Apps.Instance
{
    /// <summary>
    /// Base class for generated application facades.
    /// </summary>
    [ReferencedBySourceGenerator]
    public abstract class AppFacadeBase : IRunnable, IDisposable
    {
        private readonly IApp _app;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppFacadeBase"/> class.
        /// </summary>
        /// <param name="app">The running application instance used to execute requests.</param>
        [ReferencedBySourceGenerator]
        public AppFacadeBase(IApp app)
        {
            _app = app;
        }

        /// <summary>
        /// Executes a request that does not return a response.
        /// </summary>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <param name="requestData">The request payload.</param>
        /// <param name="cancellationToken">A token that can cancel request execution.</param>
        /// <returns>A task that completes when the request has been handled.</returns>
        [ReferencedBySourceGenerator]
        protected Task ExecuteAsync<TRequest>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest
        {
            requestData.ThrowIfNull();
            return _app.RequestBroker.ExecuteAsync(requestData, cancellationToken);
        }

        /// <summary>
        /// Executes a request that returns a response.
        /// </summary>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <param name="requestData">The request payload.</param>
        /// <param name="cancellationToken">A token that can cancel request execution.</param>
        /// <returns>A task that produces the request response.</returns>
        [ReferencedBySourceGenerator]
        protected Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            requestData.ThrowIfNull();
            return _app.RequestBroker.ExecuteAsync<TRequest, TResponse>(requestData, cancellationToken);
        }

        /// <inheritdoc/>
        public Task StartAsync(CancellationToken cancellationToken = default) => _app.StartAsync(cancellationToken);

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken = default) => _app.StopAsync(cancellationToken);

        /// <inheritdoc/>
        public void Dispose() => _app.Dispose();
    }
}
