using Tycho.Requests;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithDownstreamContract.Requests;

public class GetItemQuery : IRequest<GetItemQuery.Result> 
{ 
    public class Result { }
}
