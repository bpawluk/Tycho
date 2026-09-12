using Tycho.Requests;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.Requests;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.Handlers;

internal class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand>
{
    public Task HandleAsync(DeleteItemCommand command, CancellationToken ct) => throw new NotImplementedException();
}
