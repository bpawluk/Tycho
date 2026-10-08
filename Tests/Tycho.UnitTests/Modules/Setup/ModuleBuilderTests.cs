using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Tycho.Events.Broker;
using Tycho.Identity.Structure;
using Tycho.Modules.Instance;
using Tycho.Modules.Setup;
using Tycho.Requests.Broker;
using Tycho.Structure;
using Tycho.Structure.Parent;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Modules.Setup;

public sealed class ModuleBuilderTests
{
    private readonly ModuleBuilder _sut = new ModuleBuilder(typeof(TestModule)).WithControlPlane(new ControlPlane(InstanceIdentity.Create(typeof(ModuleBuilderTests))));
    private readonly IRequestBroker _requestBroker = Mock.Of<IRequestBroker>();
    private readonly IEventBroker _eventBroker = Mock.Of<IEventBroker>();

    [Fact]
    public void Build_WhenAlreadyBuilt_ThrowsWithoutCreatingAnotherHost()
    {
        // Arrange
        ConfigureParent();
        int hostCalls = 0;
        _sut.WithHostBuilder(() =>
        {
            hostCalls++;
            return Host.CreateEmptyApplicationBuilder(null);
        });
        using IModule module = _sut.Build();

        // Act
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => _sut.Build());

        // Assert
        Assert.Equal("The module has already been built.", exception.Message);
        Assert.Equal(1, hostCalls);
    }

    [Theory]
    [InlineData("RequestBroker")]
    [InlineData("EventBroker")]
    [InlineData("ControlPlane")]
    public void Build_WithMissingParentComponent_ThrowsBeforeCreatingHost(string missingComponent)
    {
        // Arrange
        int hostCalls = 0;
        int configurationCalls = 0;
        var sut = new ModuleBuilder(typeof(TestModule));
        if (missingComponent != "ControlPlane")
        {
            sut.WithControlPlane(new ControlPlane(InstanceIdentity.Create(typeof(ModuleBuilderTests))));
        }
        sut.WithHostBuilder(() =>
        {
            hostCalls++;
            return Host.CreateEmptyApplicationBuilder(null);
        });
        sut.WithContract(_ => configurationCalls++, missingComponent == "RequestBroker" ? null : _requestBroker);
        sut.WithEvents(_ => configurationCalls++, missingComponent == "EventBroker" ? null : _eventBroker);

        // Act
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => sut.Build());

        // Assert
        Assert.Equal("The module parent has not been configured.", exception.Message);
        Assert.Equal(0, hostCalls);
        Assert.Equal(0, configurationCalls);
    }

    [Fact]
    public void Build_WithoutHostBuilder_ThrowsBeforeConfiguringModule()
    {
        // Arrange
        int configurationCalls = 0;
        _sut.WithContract(_ => configurationCalls++, _requestBroker);
        _sut.WithEvents(_ => configurationCalls++, _eventBroker);
        _sut.WithStructure(_ => configurationCalls++);
        _sut.WithServices(_ => configurationCalls++);
        _sut.WithHostConfiguration((_, _) => configurationCalls++);

        // Act
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => _sut.Build());

        // Assert
        Assert.Equal("The module host builder has not been configured.", exception.Message);
        Assert.Equal(0, configurationCalls);
    }

    [Fact]
    public void Build_WithoutOptionalConfiguration_CreatesUsableModuleWithParentReference()
    {
        // Arrange
        ConfigureParent();
        _sut.WithHostBuilder(() => Host.CreateEmptyApplicationBuilder(null));

        // Act
        using IModule module = _sut.Build();

        // Assert
        Assert.IsType<IModule<TestModule>>(module, exactMatch: false);
        Assert.Same(module.Internals, module.Internals.GetRequiredService<Internals>());
        Assert.NotNull(module.Internals.GetRequiredService<IHostEnvironment>());
        Assert.Equal(InstanceIdentity.Create(typeof(TestModule)), module.Internals.OwnerId);
        IParentReference parent = module.Internals.GetRequiredService<IParentReference>();
        Assert.Same(_requestBroker, parent.RequestBroker);
        Assert.Same(_eventBroker, parent.EventBroker);
    }

    [Theory]
    [InlineData("WithHostBuilder", "createHostBuilder")]
    [InlineData("WithHostConfiguration", "configureHost")]
    [InlineData("WithContract", "configureContract")]
    [InlineData("WithEvents", "configureEvents")]
    [InlineData("WithStructure", "configureStructure")]
    [InlineData("WithServices", "registerServices")]
    [InlineData("WithStartup", "startup")]
    [InlineData("WithCleanup", "cleanup")]
    public void WithConfiguration_WithNullDelegate_ThrowsArgumentNullException(string method, string parameter)
    {
        // Act
        ModuleBuilder Act() => method switch
        {
            "WithHostBuilder" => _sut.WithHostBuilder(null!),
            "WithHostConfiguration" => _sut.WithHostConfiguration(null!),
            "WithContract" => _sut.WithContract(null!, _requestBroker),
            "WithEvents" => _sut.WithEvents(null!, _eventBroker),
            "WithStructure" => _sut.WithStructure(null!),
            "WithServices" => _sut.WithServices(null!),
            "WithStartup" => _sut.WithStartup(null!),
            "WithCleanup" => _sut.WithCleanup(null!),
            _ => throw new ArgumentOutOfRangeException(nameof(method))
        };

        // Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => Act());
        Assert.Equal(parameter, exception.ParamName);
    }

    [Fact]
    public void WithConfiguration_WithValidValues_ReturnsSameBuilder()
    {
        // Act & Assert
        Assert.Same(_sut, _sut.WithHostBuilder(() => Host.CreateEmptyApplicationBuilder(null)));
        Assert.Same(_sut, _sut.WithHostConfiguration((_, _) => { }));
        Assert.Same(_sut, _sut.WithContract(_ => { }, _requestBroker));
        Assert.Same(_sut, _sut.WithEvents(_ => { }, _eventBroker));
        Assert.Same(_sut, _sut.WithStructure(_ => { }));
        Assert.Same(_sut, _sut.WithServices(_ => { }));
        Assert.Same(_sut, _sut.WithStartup((_, _) => Task.CompletedTask));
        Assert.Same(_sut, _sut.WithCleanup((_, _) => Task.CompletedTask));
    }

    private void ConfigureParent()
    {
        _sut.WithContract(_ => { }, _requestBroker);
        _sut.WithEvents(_ => { }, _eventBroker);
    }
}
