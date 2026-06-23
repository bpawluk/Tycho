using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Requests;
using Tycho.Utils;

namespace Tycho.Apps.Instance
{
    /// <summary>
    /// Base class for generated Application facades.
    /// </summary>
    [ReferencedBySourceGenerator]
    public abstract class AppFacadeBase : IAsyncDisposable
    {
        private readonly IApp _app;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppFacadeBase"/> class.
        /// </summary>
        /// <param name="app">The running Application instance used to execute Requests.</param>
        [ReferencedBySourceGenerator]
        public AppFacadeBase(IApp app)
        {
            _app = app;
        }

        /// <summary>
        /// Executes a Request that does not return a Response.
        /// </summary>
        /// <typeparam name="TRequest">The Request type.</typeparam>
        /// <param name="requestData">The Request payload.</param>
        /// <param name="cancellationToken">A token that can cancel Request execution.</param>
        /// <returns>A task that completes when the Request has been handled.</returns>
        [ReferencedBySourceGenerator]
        protected Task ExecuteAsync<TRequest>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest
        {
            requestData.ThrowIfNull();
            return _app.RequestBroker.ExecuteAsync(requestData, cancellationToken);
        }

        /// <summary>
        /// Executes a Request that returns a Response.
        /// </summary>
        /// <typeparam name="TRequest">The Request type.</typeparam>
        /// <typeparam name="TResponse">The Response type.</typeparam>
        /// <param name="requestData">The Request payload.</param>
        /// <param name="cancellationToken">A token that can cancel Request execution.</param>
        /// <returns>A task that produces the Request response.</returns>
        [ReferencedBySourceGenerator]
        protected Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            requestData.ThrowIfNull();
            return _app.RequestBroker.ExecuteAsync<TRequest, TResponse>(requestData, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync() => _app.DisposeAsync();
    }
}
