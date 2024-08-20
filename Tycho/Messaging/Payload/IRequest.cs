namespace Tycho.Messaging.Payload
{
    /// <summary>
    /// An interface that represents a command message
    /// </summary>
    public interface IRequest { }

    /// <summary>
    /// An interface that represents a query message
    /// </summary>
    /// <typeparam name="Response">The type of the query response</typeparam>
    public interface IRequest<out Response> { }
}
