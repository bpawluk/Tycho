using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Requests;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithDownstreamContract.Requests;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithDownstreamContract.Handlers;

internal class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand>
{
    public Task HandleAsync(DeleteItemCommand command, CancellationToken ct) => throw new NotImplementedException();
}
