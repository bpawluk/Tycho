using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules;
using TychoV2.Apps;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving;

public class ServiceRegistrationAndResolvingTests : IAsyncLifetime
{
    private IApp _sut = null!;

    public async Task InitializeAsync()
    {
        _sut = await new TestApp().Run();
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ResolvingSingletonServices_InApps()
    {
        // Arrange
        // - no arrangement required

        // Act
        var firstResult = await _sut.Execute<GetAppSingletonServiceUsageRequest, int>(new());
        var secondResult = await _sut.Execute<GetAppSingletonServiceUsageRequest, int>(new());

        // Assert
        Assert.Equal(1, firstResult);
        Assert.Equal(2, secondResult);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ResolvingSingletonServices_InModules()
    {
        // Arrange
        // - no arrangement required

        // Act
        var firstResult = await _sut.Execute<GetModuleSingletonServiceUsageRequest, int>(new());
        var secondResult = await _sut.Execute<GetModuleSingletonServiceUsageRequest, int>(new());

        // Assert
        Assert.Equal(1, firstResult);
        Assert.Equal(2, secondResult);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ResolvingScopedServices_InApps()
    {
        // Arrange
        // - no arrangement required

        // Act
        var firstResult = await _sut.Execute<GetAppScopedServiceUsageRequest, int>(new());
        var secondResult = await _sut.Execute<GetAppScopedServiceUsageRequest, int>(new());

        // Assert
        Assert.Equal(1, firstResult);
        Assert.Equal(2, secondResult);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ResolvingScopedServices_InModules()
    {
        // Arrange
        // - no arrangement required

        // Act
        var firstResult = await _sut.Execute<GetModuleScopedServiceUsageRequest, int>(new());
        var secondResult = await _sut.Execute<GetModuleScopedServiceUsageRequest, int>(new());

        // Assert
        Assert.Equal(1, firstResult);
        Assert.Equal(2, secondResult);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ResolvingTransientServices_InApps()
    {
        // Arrange
        // - no arrangement required

        // Act
        var firstResult = await _sut.Execute<GetAppTransientServiceUsageRequest, int>(new());
        var secondResult = await _sut.Execute<GetAppTransientServiceUsageRequest, int>(new());

        // Assert
        Assert.Equal(1, firstResult);
        Assert.Equal(1, secondResult);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ResolvingTransientServices_InModules()
    {
        // Arrange
        // - no arrangement required

        // Act
        var firstResult = await _sut.Execute<GetModuleTransientServiceUsageRequest, int>(new());
        var secondResult = await _sut.Execute<GetModuleTransientServiceUsageRequest, int>(new());

        // Assert
        Assert.Equal(1, firstResult);
        Assert.Equal(1, secondResult);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
