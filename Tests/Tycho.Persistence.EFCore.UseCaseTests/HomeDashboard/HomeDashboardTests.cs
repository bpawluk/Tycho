using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation.Contract;
using Tycho.Persistence.EFCore.UseCaseTests._Utils;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard;

public sealed class HomeDashboardTests : IAsyncLifetime
{
    private readonly TestData _testData = new();
    private IHomeDashboardApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = await new HomeDashboardApp().RunAsync();
    }

    [Fact(Timeout = 10000)]
    public async Task TychoUseCase_HomeDashboardApp_WorksCorrectly()
    {
        await SetReadings();

        await AssertEventually.True(async () =>
        {
            GetTemperatureReadingsRequest.Response response = await _sut.ExecuteAsync(
                new GetTemperatureReadingsRequest(), TestContext.Current.CancellationToken);
            return _testData.GetTemperatureReadings().Match(response);
        });

        await AssertEventually.True(async () =>
        {
            GetAirQualityReadingsRequest.Response response = await _sut.ExecuteAsync(
                new GetAirQualityReadingsRequest(), TestContext.Current.CancellationToken);
            return _testData.GetAirQualityReadings().Match(response);
        });

        await AssertEventually.True(async () =>
        {
            GetSecurityEventsRequest.Response response = await _sut.ExecuteAsync(
                new GetSecurityEventsRequest(), TestContext.Current.CancellationToken);
            return _testData.GetSecurityEvents().Match(response);
        });
    }

    private async Task SetReadings()
    {
        foreach (TestData.SensorReading reading in _testData.InitialReadings)
        {
            var request = new SetReadingRequest(reading.SensorId, reading.Reading, reading.RecordedAt);
            await _sut.ExecuteAsync(request, TestContext.Current.CancellationToken);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}
