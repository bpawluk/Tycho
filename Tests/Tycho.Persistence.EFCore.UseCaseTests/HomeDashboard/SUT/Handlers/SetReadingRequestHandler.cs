using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Handlers;

internal class SetReadingRequestHandler(IHomeDashboardApp.IPublisher publisher) : ITransactionalRequestHandler<SetReadingRequest>
{
    public Task HandleAsync(SetReadingRequest requestData, CancellationToken cancellationToken)
    {
        return requestData.Reading switch
        {
            TemperatureReading reading => publisher.PublishAsync(
                new SensorEvent<TemperatureReading>(requestData.SensorId, reading, requestData.RecordedAt),
                cancellationToken),
            AirQualityReading reading => publisher.PublishAsync(
                new SensorEvent<AirQualityReading>(requestData.SensorId, reading, requestData.RecordedAt),
                cancellationToken),
            MotionDetected reading => publisher.PublishAsync(
                new SensorEvent<MotionDetected>(requestData.SensorId, reading, requestData.RecordedAt),
                cancellationToken),
            DoorOpened reading => publisher.PublishAsync(
                new SensorEvent<DoorOpened>(requestData.SensorId, reading, requestData.RecordedAt),
                cancellationToken),
            _ => throw new NotSupportedException($"Reading type {requestData.Reading.GetType().Name} is not supported.")
        };
    }
}
