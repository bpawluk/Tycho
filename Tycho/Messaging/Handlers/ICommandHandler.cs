using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Handlers
{
    public interface ICommandHandler { }

    public interface ICommandHandler<in Command> : ICommandHandler
        where Command : class, ICommand
    {
        Task Handle(Command commandData, CancellationToken cancellationToken = default);
    }
}
