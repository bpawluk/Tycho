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
                consumer.Requests.Ignore<GetDataFromThisModulesClientQuery, string>(
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
        var firstResult = await _sut!.Execute<SingletonServiceWorkflowQuery, int>(new());
        var secondResult = await _sut.Execute<SingletonServiceWorkflowQuery, int>(new());

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
        var firstResult = await _sut!.Execute<TransientServiceWorkflowQuery, int>(new());
        var secondResult = await _sut.Execute<TransientServiceWorkflowQuery, int>(new());

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
        var result = await _sut!.Execute<SubmoduleResolvingWorkflowQuery, string>(new());

        // Assert
        Assert.Equal($"Response from {typeof(AppSubmodule).Name}", result);
    }

    [Fact]
    public async Task Tycho_Enables_ResolvingTheModuleItself()
    {
        // Arrange
        // - no arrangement required

        // Act
        var result = await _sut!.Execute<ModuleResolvingWorkflowQuery, string>(new());

        // Assert
        Assert.Equal($"Response from {typeof(ServiceRegistrationAndResolvingTests).Name}", result);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
