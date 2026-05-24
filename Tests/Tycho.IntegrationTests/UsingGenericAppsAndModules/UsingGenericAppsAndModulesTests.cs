using Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT;

namespace Tycho.IntegrationTests.UsingGenericAppsAndModules;

public sealed class UsingGenericAppsAndModulesTests : IAsyncLifetime
{
    private ITestApp<SamplePayload, Guid> _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = await new TestApp<SamplePayload, Guid>().RunAsync();
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_GenericAppDefinitionsWithConstraints()
    {
        // Arrange
        var result = new TestResult { Id = "generic-app-flow" };

        // Act
        string response = await _sut.ExecuteAsync(new AppWorkflowRequest(result), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("Test = Passed", response);
        Assert.Equal(3, result.HandlingCount);
        Assert.Equal("module-parent-chain", result.LastHandledBy);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_GenericModuleDefinitionsWithConstraints()
    {
        // Arrange
        var result = new TestResult { Id = "generic-module-flow" };

        // Act
        string response = await _sut.ExecuteAsync(new ModuleWorkflowRequest(result), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("Test = Passed", response);
        Assert.Equal(2, result.HandlingCount);
        Assert.Equal("module-parent-chain", result.LastHandledBy);
    }

    public async ValueTask DisposeAsync()
    {
        await _sut.DisposeAsync();
    }
}
