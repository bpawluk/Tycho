using System.Threading.Tasks;
using IntegrationTests.ServiceRegistrationAndResolving.SUT;
using IntegrationTests.ServiceRegistrationAndResolving.SUT.Submodules;
using Tycho;

namespace IntegrationTests.ServiceRegistrationAndResolving;

public class ServiceRegistrationAndResolvingTests : IAsyncLifetime
{
    private IModule? _sut;

    public async Task InitializeAsync()
    {
        _sut = await new AppModule()
            .FulfillContract(consumer =>
            {
                consumer.Requests.Ignore<GetDataFromThisModulesClientRequestWithResponse, string>(
                    $"Response from {typeof(ServiceRegistrationAndResolvingTests).Name}");
            })
            .Build();
    }

    [Fact]
    public async Task Tycho_Enables_ResolvingSingletonServices()
    {
        // Arrange
        // - no arrangement required

        // Act
        var firstResult = await _sut!.Execute<SingletonServiceWorkflowRequest, int>(new());
        var secondResult = await _sut.Execute<SingletonServiceWorkflowRequest, int>(new());

        // Assert
        Assert.Equal(1, firstResult);
        Assert.Equal(2, secondResult);
    }

    [Fact]
    public async Task Tycho_Enables_ResolvingTransientServices()
    {
        // Arrange
        // - no arrangement required

        // Act
        var firstResult = await _sut!.Execute<TransientServiceWorkflowRequest, int>(new());
        var secondResult = await _sut.Execute<TransientServiceWorkflowRequest, int>(new());

        // Assert
        Assert.Equal(1, firstResult);
        Assert.Equal(1, secondResult);
    }

    [Fact]
    public async Task Tycho_Enables_ResolvingSubmodules()
    {
        // Arrange
        // - no arrangement required

        // Act
        var result = await _sut!.Execute<SubmoduleResolvingWorkflowRequest, string>(new());

        // Assert
        Assert.Equal($"Response from {typeof(AppSubmodule).Name}", result);
    }

    [Fact]
    public async Task Tycho_Enables_ResolvingTheModuleItself()
    {
        // Arrange
        // - no arrangement required

        // Act
        var result = await _sut!.Execute<ModuleResolvingWorkflowRequest, string>(new());

        // Assert
        Assert.Equal($"Response from {typeof(ServiceRegistrationAndResolvingTests).Name}", result);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
