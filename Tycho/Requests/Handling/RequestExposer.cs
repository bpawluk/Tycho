﻿using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure;

namespace Tycho.Requests.Handling
{
    internal class RequestExposer<TRequest> : IRequestHandler<TRequest>
        where TRequest : class, IRequest
    {
        private readonly IParent _parent;

        public RequestExposer(IParent parent)
        {
            _parent = parent;
        }

        public Task Handle(TRequest requestData, CancellationToken cancellationToken)
        {
            return _parent.Execute(requestData, cancellationToken);
        }
    }

    internal class RequestExposer<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        private readonly IParent _parent;

        public RequestExposer(IParent parent)
        {
            _parent = parent;
        }

        public Task<TResponse> Handle(TRequest requestData, CancellationToken cancellationToken)
        {
            return _parent.Execute<TRequest, TResponse>(requestData, cancellationToken);
        }
    }
}