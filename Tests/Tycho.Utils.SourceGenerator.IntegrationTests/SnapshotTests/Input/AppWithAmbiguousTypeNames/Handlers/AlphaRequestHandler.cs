using Tycho.Requests;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithAmbiguousTypeNames.Alpha;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithAmbiguousTypeNames.Handlers;

internal class AlphaRequestHandler : IRequestHandler<Request>
{
    public Task HandleAsync(Request request, CancellationToken ct) => throw new NotImplementedException();
}
