using Tycho.Requests;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithDownstreamContract.Requests;

public class GetItemQuery : IRequest<GetItemQuery.Result> 
{ 
    public class Result { }
}
