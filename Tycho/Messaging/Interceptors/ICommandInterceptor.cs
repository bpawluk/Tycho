using System.Threading.Tasks;
using System.Threading;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Interceptors
{
    /// <summary>
    /// Lets you run additional logic before and after forwarding a command message
    /// </summary>
    /// <typeparam name="Command">The type of the command being intercepted</typeparam>
    public interface ICommandInterceptor<Command> where Command : class, ICommand
    {
        /// <summary>
        /// A method to be executed before the specified command is forwarded
        /// </summary>
        /// <param name="commandData">An object that represents the command being intercepted</param>
        /// <param name="cancellationToken">A token that notifies when the operation should be canceled</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ExecuteBefore(Command commandData, CancellationToken cancellationToken = default);

        /// <summary>
        /// A method to be executed after the specified command is forwarded
        /// </summary>
        /// <param name="commandData">An object that represents the command being intercepted</param>
        /// <param name="cancellationToken">A token that notifies when the operation should be canceled</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ExecuteAfter(Command commandData, CancellationToken cancellationToken = default);
    }
}
