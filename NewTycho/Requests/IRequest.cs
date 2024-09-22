﻿using NewTycho.Messaging;

namespace NewTycho.Requests
{
    /// <summary>
    /// An interface that represents a request
    /// </summary>
    public interface IRequest : IMessage { }

    /// <summary>
    /// An interface that represents a request with a response
    /// </summary>
    /// <typeparam name="TResponse">Type of the response</typeparam>
    public interface IRequest<out TResponse> : IMessage { }
}