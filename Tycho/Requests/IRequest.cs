namespace Tycho.Requests
{
    /// <summary>
    /// An interface that represents a Request.
    /// </summary>
    public interface IRequest
    {
    }

    /// <summary>
    /// An interface that represents a Request with a response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public interface IRequest<out TResponse>
    {
    }
}
