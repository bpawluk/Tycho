using Tycho.IntegrationTests.RunningStartupLogic.SUT;
using Tycho.IntegrationTests.RunningStartupLogic.SUT.Modules;

namespace Tycho.IntegrationTests.RunningStartupLogic;

public class RunningStartupLogicTests : IAsyncLifetime
{
    private ITestApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = await new TestApp().RunAsync();
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_RunningStartupLogic_InApps()
    {
        // Arrange
        // - no arrangement required

        // Act
        var appValue = await _sut.ExecuteAsync(new GetAppValueRequest());

        // Assert
        Assert.Equal("Test = Passed", appValue);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_RunningStartupLogic_InModules()
    {
        // Arrange
        // - no arrangement required

        // Act
        var moduleValue = await _sut.ExecuteAsync(new GetModuleValueRequest());

        // Assert
        Assert.Equal("Test = Passed", moduleValue);
    }

    public async ValueTask DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}