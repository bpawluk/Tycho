using Moq;
using Tycho.Events;
using Tycho.Events.Model;
using Tycho.Identity.Events;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Model;

public class EventTests
{
    [Fact]
    public void GetHandlerFrom_WithProvider_ReturnsHandlerResolvedByHandlerId()
    {
        // Arrange
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        Event<TestEvent> sut = CreateEvent(handlerId: handlerId);

        var handlerMock = new Mock<IEventHandler<TestEvent>>();

        var providerMock = new Mock<IEventHandlerProvider>();
        providerMock.Setup(p => p.GetHandler<TestEvent>(handlerId))
                    .Returns(handlerMock.Object);

        // Act
        IEventHandler result = sut.GetHandlerFrom(providerMock.Object);

        // Assert
        Assert.Same(handlerMock.Object, result);
        providerMock.Verify(p => p.GetHandler<TestEvent>(handlerId), Times.Once);
    }

    [Fact]
    public async Task HandleWith_WithTypedHandler_InvokesHandleAsyncWithEventContext()
    {
        // Arrange
        var id = Guid.NewGuid();
        var payload = new TestEvent();
        var cancellationToken = new CancellationToken();
        Event<TestEvent> sut = CreateEvent(id: id, payload: payload);

        var handlerMock = new Mock<IEventHandler<TestEvent>>();
        handlerMock.Setup(h => h.HandleAsync(It.IsAny<EventContext<TestEvent>>(), cancellationToken))
                   .Returns(Task.CompletedTask);

        // Act
        await sut.HandleWith(handlerMock.Object, cancellationToken);

        // Assert
        handlerMock.Verify(
            h => h.HandleAsync(
                It.Is<EventContext<TestEvent>>(c => c.Id == id && c.Payload == payload),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWith_WithDifferentHandlerType_ThrowsArgumentException()
    {
        // Arrange
        Event<TestEvent> sut = CreateEvent();
        var otherHandlerMock = new Mock<IEventHandler<OtherEvent>>();

        // Act
        Task Act() => sut.HandleWith(otherHandlerMock.Object, new CancellationToken());

        // Assert
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(Act);
        Assert.Contains("IEventHandler<TestEvent>", exception.Message);
    }

    private static Event<TestEvent> CreateEvent(
        Guid? id = null,
        Guid? publishId = null,
        EventHandlerIdentity? handlerId = null,
        TestEvent? payload = null)
    {
        var eventId = EventIdentity.Create<TestEvent>();
        return new Event<TestEvent>(
            id ?? Guid.NewGuid(),
            publishId ?? Guid.NewGuid(),
            eventId,
            handlerId ?? EventHandlerIdentity.Create<TestEventHandler>(),
            payload ?? new TestEvent());
    }
}
