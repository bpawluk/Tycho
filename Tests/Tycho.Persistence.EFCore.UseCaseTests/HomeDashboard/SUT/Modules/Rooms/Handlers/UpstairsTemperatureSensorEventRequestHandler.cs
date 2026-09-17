using Tycho.Requests;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Rooms.Contract;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Rooms.Handlers;

internal class UpstairsTemperatureSensorEventRequestHandler(IUpstairsModulePublisher publisher)
    : IRequestHandler<RegisterTemperatureSensorEventRequest>
{
    public Task HandleAsync(RegisterTemperatureSensorEventRequest requestData, CancellationToken cancellationToken)
    {
        return publisher.PublishAsync(requestData.SensorEvent, cancellationToken);
    }
}
