using System;
using Tycho.Requests;
using Tycho.Utils;

namespace Tycho.Modules
{
    /// <summary>
    /// An interface for declaring the contract of a Tycho module.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IModuleContract
    {
        /// <summary>
        /// Declares that the module expects requests of type <typeparamref name="TRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the expected request.</typeparam>
        /// <returns>An expectation builder for the request.</returns>
        [ReferencedBySourceGenerator]
        IModuleRequestExpectation<TRequest> Expects<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Declares that the module expects requests of type <typeparamref name="TRequest"/> with response <typeparamref name="TResponse"/>.
        /// </summary>
        /// <typeparam name="TRequest">The type of the expected request.</typeparam>
        /// <typeparam name="TResponse">The type of the request response.</typeparam>
        /// <returns>An expectation builder for the request.</returns>
        [ReferencedBySourceGenerator]
        IModuleRequestExpectation<TRequest, TResponse> Expects<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;

        /// <summary>
        /// Declares that the module will execute requests of type <typeparamref name="TRequest"/> and requires them to be handled by its parent.
        /// </summary>
        /// <typeparam name="TRequest">The type of the required request.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract Requires<TRequest>()
            where TRequest : class, IRequest;

        /// <summary>
        /// Declares that the module will execute requests of type <typeparamref name="TRequest"/> and requires them to be handled by its parent.
        /// </summary>
        /// <typeparam name="TRequest">The type of the required request.</typeparam>
        /// <typeparam name="TResponse">The type of the required request response.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract Requires<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;
    }

    /// <summary>
    /// Configures how an expected module request is handled or forwarded.
    /// </summary>
    /// <typeparam name="TRequest">The type of the expected request.</typeparam>
    [ReferencedBySourceGenerator]
    public interface IModuleRequestExpectation<TRequest>
        where TRequest : class, IRequest
    {
        /// <summary>
        /// Declares that the module will handle the expected request using the handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="THandler">The type of request handler.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract HandlesWith<THandler>()
            where THandler : class, IRequestHandler<TRequest>;

        /// <summary>
        /// Forwards the expected request to the module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the module that receives the request.</typeparam>
        IModuleContract ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Maps the expected request to <typeparamref name="TTargetRequest"/> before forwarding.
        /// </summary>
        /// <typeparam name="TTargetRequest">The target request type.</typeparam>
        /// <param name="mapRequest">The request mapper.</param>
        IModuleMappedRequestExpectation<TRequest, TTargetRequest> MapsTo<TTargetRequest>(
            Func<TRequest, TTargetRequest> mapRequest)
            where TTargetRequest : class, IRequest;
    }

    /// <summary>
    /// Configures how an expected module request with a response is handled or forwarded.
    /// </summary>
    /// <typeparam name="TRequest">The type of the expected request.</typeparam>
    /// <typeparam name="TResponse">The type of the request response.</typeparam>
    [ReferencedBySourceGenerator]
    public interface IModuleRequestExpectation<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        /// <summary>
        /// Declares that the module will handle the expected request using the handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="THandler">The type of request handler.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleContract HandlesWith<THandler>()
            where THandler : class, IRequestHandler<TRequest, TResponse>;

        /// <summary>
        /// Forwards the expected request to the module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the module that receives the request.</typeparam>
        IModuleContract ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Maps the expected request and target response before forwarding.
        /// </summary>
        /// <typeparam name="TTargetRequest">The target request type.</typeparam>
        /// <typeparam name="TTargetResponse">The target request response type.</typeparam>
        /// <param name="mapRequest">The request mapper.</param>
        /// <param name="mapResponse">The response mapper.</param>
        IModuleMappedRequestExpectation<TRequest, TResponse, TTargetRequest, TTargetResponse> MapsTo<TTargetRequest, TTargetResponse>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TTargetRequest : class, IRequest<TTargetResponse>;
    }

    /// <summary>
    /// Configures forwarding for a mapped module request.
    /// </summary>
    /// <typeparam name="TRequest">The type of the expected request.</typeparam>
    /// <typeparam name="TTargetRequest">The target request type.</typeparam>
    public interface IModuleMappedRequestExpectation<TRequest, TTargetRequest>
        where TRequest : class, IRequest
        where TTargetRequest : class, IRequest
    {
        /// <summary>
        /// Forwards the mapped request to the module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the module that receives the mapped request.</typeparam>
        IModuleContract ForwardsTo<TModule>()
            where TModule : TychoModule;
    }

    /// <summary>
    /// Configures forwarding for a mapped module request with a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the expected request.</typeparam>
    /// <typeparam name="TResponse">The type of the request response.</typeparam>
    /// <typeparam name="TTargetRequest">The target request type.</typeparam>
    /// <typeparam name="TTargetResponse">The target request response type.</typeparam>
    public interface IModuleMappedRequestExpectation<TRequest, TResponse, TTargetRequest, TTargetResponse>
        where TRequest : class, IRequest<TResponse>
        where TTargetRequest : class, IRequest<TTargetResponse>
    {
        /// <summary>
        /// Forwards the mapped request to the module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the module that receives the mapped request.</typeparam>
        IModuleContract ForwardsTo<TModule>()
            where TModule : TychoModule;
    }
}
