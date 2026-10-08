using Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT;
using Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT.Contract;
using Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT.Modules;

namespace Tycho.IntegrationTests.UsingGenericAppsAndModules;

public sealed class UsingGenericAppsAndModulesTests : IAsyncLifetime
{
    private ITestApp<AppInput, AppOutput> _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = new TestApp<AppInput, AppOutput>().CreateAppBuilder().Build();
        await _sut.StartAsync(TestContext.Current.CancellationToken);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_GenericAppDefinitionsWithConstraints()
    {
        // Arrange
        var result = new TestResult { Id = "generic-app-flow" };

        // Act
        string response = await _sut.ExecuteAsync(new AppWorkflowRequest(result), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("Test = Passed in App<AppInput, AppOutput>", response);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_GenericModuleDefinitionsWithConstraints()
    {
        // Arrange
        var result = new TestResult { Id = "generic-module-flow" };

        // Act
        string response = await _sut.ExecuteAsync(new ModuleWorkflowRequest(result), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("Test = Passed in Module<ModuleInput, ModuleOutput>", response);
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
