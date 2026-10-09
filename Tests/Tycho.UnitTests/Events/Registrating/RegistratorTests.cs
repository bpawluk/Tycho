using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Tycho.Events;
using Tycho.Events.Registrating;
using Tycho.Events.Registrating.Registrations;
using Tycho.Identity.Events;
using Tycho.Modules.Instance;
using Tycho.Structure;
using Tycho.Structure.Parent;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Events.Registrating;

public sealed class RegistratorTests : IDisposable
{
    private readonly Internals _internals;
    private readonly Registrator _sut;

    public RegistratorTests()
    {
        _internals = new AppInternals(Host.CreateEmptyApplicationBuilder(default), typeof(RegistratorTests));
        _internals.GetHostBuilder().Services
                  .AddSingleton(_internals);
        _sut = new Registrator(_internals);
    }

    [Fact]
    public void ExposeEvent_NewEvent_RegistersExposer()
    {
        // Arrange
        var parentReferenceMock = new Mock<IParentReference>();
        _internals.GetHostBuilder().Services.AddSingleton(parentReferenceMock.Object);

        // Act
        _sut.ExposeEvent<TestEvent>();
        _internals.Build();

        // Assert
        IEventRegistration<TestEvent>? registration = _internals.GetService<IEventRegistration<TestEvent>>();
        Assert.NotNull(registration);
        Assert.IsType<ExposingEventRegistration<TestEvent>>(registration);
    }

    [Fact]
    public void ExposeEvent_ExistingEvent_ThrowsArgumentException()
    {
        // Arrange
        _sut.ExposeEvent<TestEvent>();

        // Act
        void Act() => _sut.ExposeEvent<TestEvent>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void ExposeEvent_NewMappedEvent_RegistersMappedExposer()
    {
        // Arrange
        var mapMock = new Mock<Func<TestEvent, OtherEvent>>();
        var parentReferenceMock = new Mock<IParentReference>();
        _internals.GetHostBuilder().Services.AddSingleton(parentReferenceMock.Object);

        // Act
        _sut.ExposeEvent<TestEvent, OtherEvent>(mapMock.Object);
        _internals.Build();

        // Assert
        IEventRegistration<TestEvent>? registration = _internals.GetService<IEventRegistration<TestEvent>>();
        Assert.NotNull(registration);
        Assert.IsType<MappedExposingEventRegistration<TestEvent, OtherEvent>>(registration);
    }

    [Fact]
    public void ExposeEvent_ExistingMappedEvent_ThrowsArgumentException()
    {
        // Arrange
        var mapMock = new Mock<Func<TestEvent, OtherEvent>>();
        _sut.ExposeEvent<TestEvent, OtherEvent>(mapMock.Object);

        // Act
        void Act() => _sut.ExposeEvent<TestEvent, OtherEvent>(mapMock.Object);

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void ForwardEvent_NewEvent_RegistersForwarder()
    {
        // Arrange
        var targetModuleMock = new Mock<IModule<TestModule>>();
        _internals.GetHostBuilder().Services.AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ForwardEvent<TestEvent, TestModule>();
        _internals.Build();

        // Assert
        IEventRegistration<TestEvent>? registration = _internals.GetService<IEventRegistration<TestEvent>>();
        Assert.NotNull(registration);
        Assert.IsType<ForwardingEventRegistration<TestEvent, TestModule>>(registration);
    }

    [Fact]
    public void ForwardEvent_ExistingEvent_ThrowsArgumentException()
    {
        // Arrange
        _sut.ForwardEvent<TestEvent, TestModule>();

        // Act
        void Act() => _sut.ForwardEvent<TestEvent, TestModule>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void ForwardEvent_NewMappedEvent_RegistersMappedForwarder()
    {
        // Arrange
        var mapMock = new Mock<Func<TestEvent, OtherEvent>>();
        var targetModuleMock = new Mock<IModule<TestModule>>();
        _internals.GetHostBuilder().Services.AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ForwardEvent<TestEvent, OtherEvent, TestModule>(mapMock.Object);
        _internals.Build();

        // Assert
        IEventRegistration<TestEvent>? registration = _internals.GetService<IEventRegistration<TestEvent>>();
        Assert.NotNull(registration);
        Assert.IsType<MappedForwardingEventRegistration<TestEvent, OtherEvent, TestModule>>(registration);
    }

    [Fact]
    public void ForwardEvent_ExistingMappedEvent_ThrowsArgumentException()
    {
        // Arrange
        var mapMock = new Mock<Func<TestEvent, OtherEvent>>();
        _sut.ForwardEvent<TestEvent, OtherEvent, TestModule>(mapMock.Object);

        // Act
        void Act() => _sut.ForwardEvent<TestEvent, OtherEvent, TestModule>(mapMock.Object);

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void HandleEvent_NewEvent_RegistersHandler()
    {
        // Arrange
        // - no arrangement required

        // Act
        _sut.HandleEvent<TestEvent, TestEventHandler>();
        _internals.Build();

        // Assert
        IEventRegistration<TestEvent>? eventRegistration = _internals.GetService<IEventRegistration<TestEvent>>();
        Assert.NotNull(eventRegistration);
        Assert.IsType<FinalEventRegistration<TestEvent, TestEventHandler>>(eventRegistration);

        IFinalEventRegistration<TestEvent>? finalEventRegistration = _internals.GetService<IFinalEventRegistration<TestEvent>>();
        Assert.NotNull(finalEventRegistration);
        Assert.IsType<FinalEventRegistration<TestEvent, TestEventHandler>>(finalEventRegistration);

        using IServiceScope scope = _internals.CreateScope();
        IEventHandler<TestEvent>? handler = scope.ServiceProvider.GetKeyedService<IEventHandler<TestEvent>>(finalEventRegistration.HandlerId);
        Assert.NotNull(handler);
        Assert.IsType<TestEventHandler>(handler);

        Assert.Equal(EventHandlerIdentity.Create<TestEventHandler>(), finalEventRegistration.HandlerId);
    }

    [Fact]
    public void HandleEvent_ExistingEvent_ThrowsArgumentException()
    {
        // Arrange
        _sut.HandleEvent<TestEvent, TestEventHandler>();

        // Act
        void Act() => _sut.HandleEvent<TestEvent, TestEventHandler>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void HandleEvent_WithSharedHandlerId_ThrowsWithoutChangingRegistrations(bool sameEvent)
    {
        // Arrange
        _sut.HandleEvent<TestEvent, FirstHandler>();
        IServiceCollection services = _internals.GetHostBuilder().Services;
        ServiceDescriptor[] originalRegistrations = [.. services];

        // Act
        void Act()
        {
            if (sameEvent)
            {
                _sut.HandleEvent<TestEvent, SecondHandler>();
            }
            else
            {
                _sut.HandleEvent<OtherEvent, SecondHandler>();
            }
        }

        // Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(Act);
        Assert.Equal("THandler", exception.ParamName);
        Assert.Contains("shared-handler", exception.Message);
        Assert.Equal(originalRegistrations, [.. services]);
    }

    [Fact]
    public void HandleEvent_WithSharedHandlerIdInDifferentModules_RegistersIndependently()
    {
        // Arrange
        using var firstModule = new ModuleInternals(
            Host.CreateEmptyApplicationBuilder(default), _internals.ControlPlane, typeof(TestModule));
        using var secondModule = new ModuleInternals(
            Host.CreateEmptyApplicationBuilder(default), _internals.ControlPlane, typeof(OtherModule));
        firstModule.GetHostBuilder().Services.AddSingleton<Internals>(firstModule);
        secondModule.GetHostBuilder().Services.AddSingleton<Internals>(secondModule);
        var firstRegistrator = new Registrator(firstModule);
        var secondRegistrator = new Registrator(secondModule);

        // Act
        firstRegistrator.HandleEvent<TestEvent, FirstHandler>();
        secondRegistrator.HandleEvent<TestEvent, SecondHandler>();
        firstModule.Build();
        secondModule.Build();

        // Assert
        using IServiceScope firstScope = firstModule.CreateScope();
        using IServiceScope secondScope = secondModule.CreateScope();
        var handlerId = EventHandlerIdentity.Create<FirstHandler>();
        Assert.IsType<FirstHandler>(new EventHandlerProvider(firstScope.ServiceProvider).GetHandler<TestEvent>(handlerId));
        Assert.IsType<SecondHandler>(new EventHandlerProvider(secondScope.ServiceProvider).GetHandler<TestEvent>(handlerId));
    }

    [Fact]
    public void HandleEvent_WithSameHandlerForDifferentEvents_RegistersBothInterfaces()
    {
        // Act
        _sut.HandleEvent<TestEvent, MultiEventHandler>();
        _sut.HandleEvent<OtherEvent, MultiEventHandler>();
        _internals.Build();

        // Assert
        using IServiceScope scope = _internals.CreateScope();
        var provider = new EventHandlerProvider(scope.ServiceProvider);
        var handlerId = EventHandlerIdentity.Create<MultiEventHandler>();
        Assert.IsType<MultiEventHandler>(provider.GetHandler<TestEvent>(handlerId));
        Assert.IsType<MultiEventHandler>(provider.GetHandler<OtherEvent>(handlerId));
    }

    [Fact]
    public void HandleEvent_WithScopedHandler_ReusesInstanceWithinScope()
    {
        // Arrange
        _sut.HandleEvent<TestEvent, TestEventHandler>();
        _internals.Build();
        using IServiceScope scope = _internals.CreateScope();
        var provider = new EventHandlerProvider(scope.ServiceProvider);
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();

        // Act
        IEventHandler<TestEvent> first = provider.GetHandler<TestEvent>(handlerId);
        IEventHandler<TestEvent> second = provider.GetHandler<TestEvent>(handlerId);

        // Assert
        Assert.Same(first, second);
    }

    [Fact]
    public void HandleEvent_WithScopedHandler_CreatesDifferentInstancesAcrossScopes()
    {
        // Arrange
        _sut.HandleEvent<TestEvent, TestEventHandler>();
        _internals.Build();
        using IServiceScope firstScope = _internals.CreateScope();
        using IServiceScope secondScope = _internals.CreateScope();
        var firstProvider = new EventHandlerProvider(firstScope.ServiceProvider);
        var secondProvider = new EventHandlerProvider(secondScope.ServiceProvider);
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();

        // Act
        IEventHandler<TestEvent> first = firstProvider.GetHandler<TestEvent>(handlerId);
        IEventHandler<TestEvent> second = secondProvider.GetHandler<TestEvent>(handlerId);

        // Assert
        Assert.NotSame(first, second);
    }

    public void Dispose() => _internals.Dispose();

    [TychoId("shared-handler")]
    private sealed class FirstHandler : IEventHandler<TestEvent>
    {
        public Task HandleAsync(EventContext<TestEvent> context, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    [TychoId("shared-handler")]
    private sealed class SecondHandler : IEventHandler<TestEvent>, IEventHandler<OtherEvent>
    {
        public Task HandleAsync(EventContext<TestEvent> context, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task HandleAsync(EventContext<OtherEvent> context, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
