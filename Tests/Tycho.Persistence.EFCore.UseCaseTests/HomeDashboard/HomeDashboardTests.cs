using Tycho.Persistence.EFCore.TestUtils.Databases.Sessions;
using Tycho.Persistence.EFCore.TestUtils.Databases;
using Tycho.Persistence.EFCore.TestUtils.Assertions;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation.Contract;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard;

public sealed class HomeDashboardTests(DatabaseServers servers) : IAsyncLifetime
{
    private DatabaseSession? _databaseSession;
    private readonly TestData _testData = new();
    private IHomeDashboardApp _sut = null!;

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    private async Task StartAppAsync()
    {
        _sut = new HomeDashboardApp().CreateAppBuilder().Build();
        await _sut.StartAsync(TestContext.Current.CancellationToken);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task TychoUseCase_HomeDashboardApp_WorksCorrectly(DatabaseProvider provider)
    {
        _databaseSession = await DatabaseSessionFactory.CreateAsync(servers, provider);
        using IDisposable binding = DatabaseSessionScope.Bind(_databaseSession);

        await StartAppAsync();
        await SetReadings();

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

        await AssertTemperatureReadings("upstairs", _testData.GetUpstairsTemperatureReadings());
        await AssertTemperatureReadings("downstairs", _testData.GetDownstairsTemperatureReadings());
    }

    private async Task AssertTemperatureReadings(string room, TestData.TemperatureReadings expected)
    {
        GetTemperatureReadingsRequest.Response? actual = null;
        try
        {
            await AssertEventually.True(async () =>
            {
                actual = await _sut.ExecuteAsync(
                    new GetRoomTemperatureReadingsRequest(room), TestContext.Current.CancellationToken);
                return expected.Match(actual);
            });
        }
        finally
        {
            if (actual is not null && !expected.Match(actual))
            {
                TestContext.Current.TestOutputHelper?.WriteLine($"Temperature readings for {room}: expected {System.Text.Json.JsonSerializer.Serialize(expected)}; actual {System.Text.Json.JsonSerializer.Serialize(actual.Readings)}");
            }
        }
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
        try
        {
            if (_sut is not null) await _sut.StopAsync();
        }
        finally
        {
            try
            {
                _sut?.Dispose();
            }
            finally
            {
                if (_databaseSession is not null) await _databaseSession.DisposeAsync();
            }
        }
    }
}
