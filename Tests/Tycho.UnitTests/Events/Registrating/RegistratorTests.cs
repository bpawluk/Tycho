using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Events.Handling;
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

public class RegistratorTests
{
    private readonly Internals _internals;
    private readonly Registrator _sut;

    public RegistratorTests()
    {
        _internals = new Internals(typeof(TestModule));
        _internals.GetServiceCollection()
                  .AddSingleton(_internals);
        _sut = new Registrator(_internals);
    }

    [Fact]
    public void Expose_NewEvent_RegistersExposer()
    {
        // Arrange
        var parentReferenceMock = new Mock<IParentReference>();
        _internals.GetServiceCollection().AddSingleton(parentReferenceMock.Object);

        // Act
        _sut.ExposeEvent<TestEvent>();
        _internals.Build();

        // Assert
        var registration = _internals.GetService<IEventRegistration<TestEvent>>();
        Assert.NotNull(registration);
        Assert.IsType<ExposingEventRegistration<TestEvent>>(registration);
    }

    [Fact]
    public void Expose_ExistingEvent_ThrowsArgumentException()
    {
        // Arrange
        _sut.ExposeEvent<TestEvent>();

        // Act
        void Act() => _sut.ExposeEvent<TestEvent>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Expose_NewMappedEvent_RegistersMappedExposer()
    {
        // Arrange
        var mapMock = new Mock<Func<TestEvent, OtherEvent>>();
        var parentReferenceMock = new Mock<IParentReference>();
        _internals.GetServiceCollection().AddSingleton(parentReferenceMock.Object);

        // Act
        _sut.ExposeEvent<TestEvent, OtherEvent>(mapMock.Object);
        _internals.Build();

        // Assert
        var registration = _internals.GetService<IEventRegistration<TestEvent>>();
        Assert.NotNull(registration);
        Assert.IsType<MappedExposingEventRegistration<TestEvent, OtherEvent>>(registration);
    }

    [Fact]
    public void Expose_ExistingMappedEvent_ThrowsArgumentException()
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
    public void Forward_NewEvent_RegistersForwarder()
    {
        // Arrange
        var targetModuleMock = new Mock<IModule<TestModule>>();
        _internals.GetServiceCollection().AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ForwardEvent<TestEvent, TestModule>();
        _internals.Build();

        // Assert
        var registration = _internals.GetService<IEventRegistration<TestEvent>>();
        Assert.NotNull(registration);
        Assert.IsType<ForwardingEventRegistration<TestEvent, TestModule>>(registration);
    }

    [Fact]
    public void Forward_ExistingEvent_ThrowsArgumentException()
    {
        // Arrange
        _sut.ForwardEvent<TestEvent, TestModule>();

        // Act
        void Act() => _sut.ForwardEvent<TestEvent, TestModule>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Forward_NewMappedEvent_RegistersMappedForwarder()
    {
        // Arrange
        var mapMock = new Mock<Func<TestEvent, OtherEvent>>();
        var targetModuleMock = new Mock<IModule<TestModule>>();
        _internals.GetServiceCollection().AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ForwardEvent<TestEvent, OtherEvent, TestModule>(mapMock.Object);
        _internals.Build();

        // Assert
        var registration = _internals.GetService<IEventRegistration<TestEvent>>();
        Assert.NotNull(registration);
        Assert.IsType<MappedForwardingEventRegistration<TestEvent, OtherEvent, TestModule>>(registration);
    }

    [Fact]
    public void Forward_ExistingMappedEvent_ThrowsArgumentException()
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
    public void Handle_NewEvent_RegistersHandler()
    {
        // Arrange
        // - no arrangement required

        // Act
        _sut.HandleEvent<TestEvent, TestEventHandler>();
        _internals.Build();

        // Assert
        var eventRegistration = _internals.GetService<IEventRegistration<TestEvent>>();
        Assert.NotNull(eventRegistration);
        Assert.IsType<FinalEventRegistration<TestEvent, ScopedEventHandler<TestEvent, TestEventHandler>>>(eventRegistration);

        var finalEventRegistration = _internals.GetService<IFinalEventRegistration<TestEvent>>();
        Assert.NotNull(finalEventRegistration);
        Assert.IsType<FinalEventRegistration<TestEvent, ScopedEventHandler<TestEvent, TestEventHandler>>>(finalEventRegistration);

        Assert.NotNull(finalEventRegistration.Handler);
        Assert.IsType<ScopedEventHandler<TestEvent, TestEventHandler>>(finalEventRegistration.Handler);

        Assert.Equal(EventHandlerIdentity.Create<TestEventHandler>(), finalEventRegistration.HandlerId);
    }

    [Fact]
    public void Handle_ExistingEvent_ThrowsArgumentException()
    {
        // Arrange
        _sut.HandleEvent<TestEvent, TestEventHandler>();

        // Act
        void Act() => _sut.HandleEvent<TestEvent, TestEventHandler>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }
}