using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Rooms.Contract;
using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Rooms.Handlers;

internal class DownstairsTemperatureSensorEventRequestHandler(IDownstairsModulePublisher publisher)
    : IRequestHandler<RegisterTemperatureSensorEventRequest>
{
    public Task HandleAsync(RegisterTemperatureSensorEventRequest requestData, CancellationToken cancellationToken)
    {
        return publisher.PublishAsync(requestData.SensorEvent, cancellationToken);
    }
}
