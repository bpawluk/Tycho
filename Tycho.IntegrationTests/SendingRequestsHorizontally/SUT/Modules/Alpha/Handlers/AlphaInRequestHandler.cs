using TychoV2.Requests;
using TychoV2.Structure;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Alpha.Handlers;

internal class AlphaInRequestHandler(IParent parent)
    : IRequestHandler<AlphaInRequest>
    , IRequestHandler<AlphaInRequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task Handle(AlphaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.Execute(new AlphaOutRequest(requestData.Result), cancellationToken);
    }

    public Task<string> Handle(AlphaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.Execute<AlphaOutRequestWithResponse, string>(
            new AlphaOutRequestWithResponse(requestData.Result), 
            cancellationToken);
    }
}
