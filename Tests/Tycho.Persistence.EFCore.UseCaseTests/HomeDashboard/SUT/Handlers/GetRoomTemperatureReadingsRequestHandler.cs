using Tycho.Requests;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Rooms;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Contract;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Handlers;

internal sealed class GetRoomTemperatureReadingsRequestHandler(IDownstairsModule downstairs, IUpstairsModule upstairs)
    : IRequestHandler<GetRoomTemperatureReadingsRequest, GetTemperatureReadingsRequest.Response>
{
    public Task<GetTemperatureReadingsRequest.Response> HandleAsync(GetRoomTemperatureReadingsRequest requestData, CancellationToken cancellationToken)
    {
        var newRequest = new GetTemperatureReadingsRequest();

        if (requestData.Room == "downstairs")
        {
            return downstairs.ExecuteAsync(newRequest, cancellationToken);
        }

        if (requestData.Room == "upstairs")
        {
            return upstairs.ExecuteAsync(newRequest, cancellationToken);
        }

        throw new ArgumentException("Unknown room.", nameof(requestData));
    }
}
