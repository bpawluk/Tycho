using IntegrationTests.ForwardingMessages.SUT.Submodules;
using System.Threading.Tasks;
using System.Threading;
using Tycho.Messaging.Interceptors;

namespace IntegrationTests.ForwardingMessages.SUT.Interceptors;

internal class CommandInterceptor :
    ICommandInterceptor<CommandToForward>,
    ICommandInterceptor<MappedAlphaCommand>,
    ICommandInterceptor<MappedBetaCommand>,
    ICommandInterceptor<MappedCommand>
{
    public Task ExecuteAfter(CommandToForward commandData, CancellationToken cancellationToken = default)
    {
        commandData.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(MappedAlphaCommand commandData, CancellationToken cancellationToken = default)
    {
        commandData.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(MappedBetaCommand commandData, CancellationToken cancellationToken = default)
    {
        commandData.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(MappedCommand commandData, CancellationToken cancellationToken = default)
    {
        commandData.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(CommandToForward commandData, CancellationToken cancellationToken = default)
    {
        commandData.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(MappedAlphaCommand commandData, CancellationToken cancellationToken = default)
    {
        commandData.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(MappedBetaCommand commandData, CancellationToken cancellationToken = default)
    {
        commandData.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(MappedCommand commandData, CancellationToken cancellationToken = default)
    {
        commandData.PreInterceptions++;
        return Task.CompletedTask;
    }
}
