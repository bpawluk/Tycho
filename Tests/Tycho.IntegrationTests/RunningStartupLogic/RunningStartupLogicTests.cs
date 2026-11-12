using Tycho.IntegrationTests.RunningStartupLogic.SUT;
using Tycho.IntegrationTests.RunningStartupLogic.SUT.Modules;

namespace Tycho.IntegrationTests.RunningStartupLogic;

public sealed class RunningStartupLogicTests : IAsyncLifetime
{
    private ITestApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = new TestApp().CreateAppBuilder().Build();
        await _sut.StartAsync(TestContext.Current.CancellationToken);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_RunningStartupLogic_InApps()
    {
        // Arrange
        // - no arrangement required

        // Act
        string appValue = await _sut.ExecuteAsync(new GetAppValueRequest(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("Test = Passed", appValue);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_RunningStartupLogic_InModules()
    {
        // Arrange
        // - no arrangement required

        // Act
        string moduleValue = await _sut.ExecuteAsync(new GetModuleValueRequest(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("Test = Passed", moduleValue);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _sut.StopAsync();
        }
        finally
        {
            _sut.Dispose();
        }
    }
}
