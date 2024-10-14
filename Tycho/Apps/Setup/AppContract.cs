﻿using System.Threading.Tasks;
using Tycho.Modules;
using Tycho.Requests;
using Tycho.Requests.Registrating;
using Tycho.Structure;

namespace Tycho.Apps.Setup
{
    internal class AppContract : IAppContract
    {
        private readonly Registrator _registrator;

        public AppContract(Internals internals)
        {
            _registrator = new Registrator(internals);
        }

        public IAppContract Forwards<TRequest, TModule>()
            where TRequest : class, IRequest
            where TModule : TychoModule
        {
            _registrator.ForwardUpStreamRequest<TRequest, TModule>();
            return this;
        }

        public IAppContract Forwards<TRequest, TResponse, TModule>()
            where TRequest : class, IRequest<TResponse>
            where TModule : TychoModule
        {
            _registrator.ForwardUpStreamRequest<TRequest, TResponse, TModule>();
            return this;
        }

        public IAppContract Handles<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>
        {
            _registrator.HandleUpStreamRequest<TRequest, THandler>();
            return this;
        }

        public IAppContract Handles<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            _registrator.HandleUpStreamRequest<TRequest, TResponse, THandler>();
            return this;
        }

        public Task Build() => Task.CompletedTask;
    }
}