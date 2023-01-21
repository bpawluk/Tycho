using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Handlers
{
    /// <summary>
    /// An interface that represents a command message handler
    /// </summary>
    /// <typeparam name="Command">The type of the command being handled</typeparam>
    public interface ICommandHandler<in Command> : ICommandHandler
        where Command : class, ICommand
    {
        /// <summary>
        /// A method to be executed when the specified command is received
        /// </summary>
        /// <param name="commandData">An object that represents the command being handled</param>
        /// <param name="cancellationToken">A token that notifies when the operation should be canceled</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task Handle(Command commandData, CancellationToken cancellationToken = default);
    }

    public interface ICommandHandler { }
}
