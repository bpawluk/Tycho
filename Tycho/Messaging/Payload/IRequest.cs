namespace Tycho.Messaging.Payload
{
    /// <summary>
    /// An interface that represents a request
    /// </summary>
    public interface IRequest { }

    /// <summary>
    /// An interface that represents a request with a response
    /// </summary>
    /// <typeparam name="Response">The type of the request response</typeparam>
    public interface IRequest<out Response> { }
}
