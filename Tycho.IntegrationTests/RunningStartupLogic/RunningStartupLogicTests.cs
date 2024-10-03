﻿using Tycho.IntegrationTests.RunningStartupLogic.SUT;
using Tycho.IntegrationTests.RunningStartupLogic.SUT.Modules;
using TychoV2.Apps;

namespace Tycho.IntegrationTests.RunningStartupLogic;

public class RunningStartupLogicTests : IAsyncLifetime
{
    private IApp _sut = null!;

    public async Task InitializeAsync()
    {
        _sut = await new TestApp().Run();
    }

    [Fact]
    public async Task TychoEnables_RunningStartupLogic_InApps()
    {
        // Arrange
        // - no arrangement required

        // Act
        var appValue = await _sut.Execute<GetAppValueRequest, string>(new());

        // Assert
        Assert.Equal("Test = Passed", appValue);
    }

    [Fact]
    public async Task TychoEnables_RunningStartupLogic_InModules()
    {
        // Arrange
        // - no arrangement required

        // Act
        var moduleValue = await _sut.Execute<GetModuleValueRequest, string>(new());

        // Assert
        Assert.Equal("Test = Passed", moduleValue);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
