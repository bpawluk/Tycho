using Tycho.Requests;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.Requests;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.Handlers;

internal class GetItemQueryHandler : IRequestHandler<GetItemQuery, GetItemQuery.Result>
{
    public Task<GetItemQuery.Result> HandleAsync(GetItemQuery query, CancellationToken ct) => throw new NotImplementedException();
}
