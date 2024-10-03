﻿using System.Threading;
using System.Threading.Tasks;

namespace TychoV2.Requests.Handling
{
    internal class RequestIgnorer<TRequest> : IHandle<TRequest>
        where TRequest : class, IRequest
    {
        public Task Handle(TRequest requestData, CancellationToken cancellationToken) => 
            Task.CompletedTask;
    }

    internal class RequestIgnorer<TRequest, TResponse> : IHandle<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        public Task<TResponse> Handle(TRequest requestData, CancellationToken cancellationToken) => 
            Task.FromResult(default(TResponse)!);
    }
}
