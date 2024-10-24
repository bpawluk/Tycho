using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules;
using Tycho.Structure;

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
        var firstResult = await _sut.Execute<GetAppSingletonServiceUsageRequest, int>(
            new GetAppSingletonServiceUsageRequest());
        var secondResult =  await _sut.Execute<GetAppSingletonServiceUsageRequest, int>(
            new GetAppSingletonServiceUsageRequest());

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
        var firstResult = await _sut.Execute<GetModuleSingletonServiceUsageRequest, int>(
            new GetModuleSingletonServiceUsageRequest());
        var secondResult = await _sut.Execute<GetModuleSingletonServiceUsageRequest, int>(
            new GetModuleSingletonServiceUsageRequest());

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
        var firstResult = await _sut.Execute<GetAppScopedServiceUsageRequest, int>(
            new GetAppScopedServiceUsageRequest());
        var secondResult = await _sut.Execute<GetAppScopedServiceUsageRequest, int>(
            new GetAppScopedServiceUsageRequest());

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
        var firstResult = await _sut.Execute<GetModuleScopedServiceUsageRequest, int>(
            new GetModuleScopedServiceUsageRequest());
        var secondResult = await _sut.Execute<GetModuleScopedServiceUsageRequest, int>(
            new GetModuleScopedServiceUsageRequest());

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
        var firstResult = await _sut.Execute<GetAppTransientServiceUsageRequest, int>(
            new GetAppTransientServiceUsageRequest());
        var secondResult = await _sut.Execute<GetAppTransientServiceUsageRequest, int>(
            new GetAppTransientServiceUsageRequest());

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
        var firstResult = await _sut.Execute<GetModuleTransientServiceUsageRequest, int>(
            new GetModuleTransientServiceUsageRequest());
        var secondResult = await _sut.Execute<GetModuleTransientServiceUsageRequest, int>(
            new GetModuleTransientServiceUsageRequest());

        // Assert
        Assert.Equal(1, firstResult);
        Assert.Equal(1, secondResult);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}