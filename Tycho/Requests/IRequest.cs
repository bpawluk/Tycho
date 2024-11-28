namespace Tycho.Requests
{
    /// <summary>
    /// An interface that represents a request
    /// </summary>
    public interface IRequest
    {
    }

    /// <summary>
    /// An interface that represents a request with response
    /// </summary>
    /// <typeparam name="TResponse">The type of the response</typeparam>
    public interface IRequest<out TResponse>
    {
    }
}