using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Events;
using Tycho.Events.Handling;
using Tycho.Events.Registrating;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Sources;
using Tycho.Structure;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Events.Registrating;

public class RegistratorTests
{
    private readonly Internals _internals;
    private readonly Registrator _sut;

    public RegistratorTests()
    {
        _internals = new Internals(typeof(object));
        _sut = new Registrator(_internals);
    }

    [Fact]
    public void Expose_NewEvent_RegistersHandlersSource()
    {
        // Arrange
        var targetModuleMock = new Mock<IParent>();
        _internals.GetServiceCollection().AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ExposeEvent<TestEvent>();
        _internals.Build();

        // Assert
        var source = _internals.GetService<IHandlersSource>();
        Assert.NotNull(source);
        Assert.IsType<UpStreamHandlersSource<TestEvent>>(source);
    }

    [Fact]
    public void Expose_ExistingEvent_ThrowsArgumentException()
    {
        // Arrange
        _internals.GetServiceCollection()
            .AddTransient<IHandlersSource, UpStreamHandlersSource<TestEvent>>();

        // Act
        void Act() => _sut.ExposeEvent<TestEvent>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Expose_NewEventWithMapping_RegistersHandlersSource()
    {
        // Arrange
        var targetModuleMock = new Mock<IParent>();
        _internals.GetServiceCollection().AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ExposeEvent<TestEvent, OtherEvent>(eventData => new());
        _internals.Build();

        // Assert
        var source = _internals.GetService<IHandlersSource>();
        Assert.NotNull(source);
        Assert.IsType<UpStreamMappedHandlersSource<TestEvent, OtherEvent>>(source);
    }

    [Fact]
    public void Expose_ExistingEventWithMapping_ThrowsArgumentException()
    {
        // Arrange
        _internals.GetServiceCollection()
            .AddTransient<IHandlersSource, UpStreamMappedHandlersSource<TestEvent, OtherEvent>>();

        // Act
        void Act() => _sut.ExposeEvent<TestEvent, OtherEvent>(eventData => new());

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Forward_NewEvent_RegistersHandlersSource()
    {
        // Arrange
        var targetModuleMock = new Mock<IModule<TestModule>>();
        _internals.GetServiceCollection().AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ForwardEvent<TestEvent, TestModule>();
        _internals.Build();

        // Assert
        var source = _internals.GetService<IHandlersSource>();
        Assert.NotNull(source);
        Assert.IsType<DownStreamHandlersSource<TestEvent, TestModule>>(source);
    }

    [Fact]
    public void Forward_ExistingEvent_ThrowsArgumentException()
    {
        // Arrange
        _internals.GetServiceCollection()
            .AddTransient<IHandlersSource, DownStreamHandlersSource<TestEvent, TestModule>>();

        // Act
        void Act() => _sut.ForwardEvent<TestEvent, TestModule>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Forward_NewEventWithMapping_RegistersHandlersSource()
    {
        // Arrange
        var targetModuleMock = new Mock<IModule<TestModule>>();
        _internals.GetServiceCollection().AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ForwardEvent<TestEvent, OtherEvent, TestModule>(eventData => new());
        _internals.Build();

        // Assert
        var source = _internals.GetService<IHandlersSource>();
        Assert.NotNull(source);
        Assert.IsType<DownStreamMappedHandlersSource<TestEvent, OtherEvent, TestModule>>(source);
    }

    [Fact]
    public void Forward_ExistingEventWithMapping_ThrowsArgumentException()
    {
        // Arrange
        _internals.GetServiceCollection()
            .AddTransient<IHandlersSource, DownStreamMappedHandlersSource<TestEvent, OtherEvent, TestModule>>();

        // Act
        void Act() => _sut.ForwardEvent<TestEvent, OtherEvent, TestModule>(eventData => new());

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Handle_NewEventAndNewHandler_RegistersHandlerAndHandlersSource()
    {
        // Arrange
        _internals.GetServiceCollection().AddSingleton(_internals);

        // Act
        _sut.HandleEvent<TestEvent, TestEventHandler>();
        _internals.Build();

        // Assert
        var source = _internals.GetService<IHandlersSource>();
        Assert.NotNull(source);
        Assert.IsType<LocalHandlersSource<TestEvent>>(source);

        var scopedHandler = _internals.GetService<IEventHandler<TestEvent>>();
        Assert.NotNull(scopedHandler);
        Assert.IsType<ScopedEventHandler<TestEvent, TestEventHandler>>(scopedHandler);

        var actualHandler = _internals.GetServices<TestEventHandler>();
        Assert.NotNull(actualHandler);
    }

    [Fact]
    public void Handle_ExistingEventAndNewHandler_RegistersHandler()
    {
        // Arrange
        _internals.GetServiceCollection()
            .AddSingleton(_internals)
            .AddTransient<IHandlersSource, LocalHandlersSource<TestEvent>>()
            .AddTransient<IEventHandler<TestEvent>, ScopedEventHandler<TestEvent, TestEventHandler>>()
            .AddScoped<TestEventHandler>();

        // Act
        _sut.HandleEvent<TestEvent, TestEventOtherHandler>();
        _internals.Build();

        // Assert
        var handlers = _internals.GetServices<IEventHandler<TestEvent>>();
        Assert.Equal(2, handlers.Count());
        Assert.Contains(handlers, handler => handler is ScopedEventHandler<TestEvent, TestEventOtherHandler>);
    }

    [Fact]
    public void Handle_ExistingEventAndExistingHandler_ThrowsArgumentException()
    {
        // Arrange
        _internals.GetServiceCollection()
            .AddSingleton(_internals)
            .AddTransient<IHandlersSource, LocalHandlersSource<TestEvent>>()
            .AddTransient<IEventHandler<TestEvent>, ScopedEventHandler<TestEvent, TestEventHandler>>()
            .AddScoped<TestEventHandler>();

        // Act
        void Act() => _sut.HandleEvent<TestEvent, TestEventHandler>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }
}