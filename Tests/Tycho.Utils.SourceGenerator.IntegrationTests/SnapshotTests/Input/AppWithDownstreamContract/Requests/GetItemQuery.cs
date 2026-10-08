using Tycho.Requests;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.Requests;

public class GetItemQuery : IRequest<GetItemQuery.Result> 
{ 
    public class Result { }
}
