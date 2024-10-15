using Tycho.Apps;
using Tycho.IntegrationTests.RunningStartupLogic.SUT;
using Tycho.IntegrationTests.RunningStartupLogic.SUT.Modules;

namespace Tycho.IntegrationTests.RunningStartupLogic;

public class RunningStartupLogicTests : IAsyncLifetime
{
    private IApp _sut = null!;

    public async Task InitializeAsync()
    {
        _sut = await new TestApp().Run();
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_RunningStartupLogic_InApps()
    {
        // Arrange
        // - no arrangement required

        // Act
        var appValue = await _sut.Execute<GetAppValueRequest, string>(new GetAppValueRequest());

        // Assert
        Assert.Equal("Test = Passed", appValue);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_RunningStartupLogic_InModules()
    {
        // Arrange
        // - no arrangement required

        // Act
        var moduleValue = await _sut.Execute<GetModuleValueRequest, string>(new GetModuleValueRequest());

        // Assert
        Assert.Equal("Test = Passed", moduleValue);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}