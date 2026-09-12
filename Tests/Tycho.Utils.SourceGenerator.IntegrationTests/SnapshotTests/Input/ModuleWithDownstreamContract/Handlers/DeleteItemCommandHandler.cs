using Tycho.Requests;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract.Requests;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract.Handlers;

internal class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand>
{
    public Task HandleAsync(DeleteItemCommand command, CancellationToken ct) => throw new NotImplementedException();
}
