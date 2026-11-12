using Tycho.Requests;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithAmbiguousTypeNames.Beta;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithAmbiguousTypeNames.Handlers;

internal class BetaRequestHandler : IRequestHandler<Request>
{
    public Task HandleAsync(Request request, CancellationToken ct) => throw new NotImplementedException();
}
