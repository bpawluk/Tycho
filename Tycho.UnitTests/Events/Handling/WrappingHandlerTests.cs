using Moq;
using Tycho.Events;
using Tycho.Events.Handling;
using Tycho.UnitTests._Data.Events;

namespace Tycho.UnitTests.Events.Handling;

public class WrappingHandlerTests
{
    [Fact]
    public async Task Handle_InvokesWrappedHandlerWithMappedEventData()
    {
        // Arrange
        var eventData = new TestEvent();
        var mappedEventData = new OtherEvent();

        var wrappedHandler = new Mock<IEventHandler<OtherEvent>>();
        var map = new Mock<Func<TestEvent, OtherEvent>>();
        map.Setup(m => m(eventData)).Returns(mappedEventData);

        var sut = new WrappingHandler<TestEvent, OtherEvent>(wrappedHandler.Object, map.Object);

        // Act
        await sut.Handle(eventData, CancellationToken.None);

        // Assert
        wrappedHandler.Verify(w => w.Handle(mappedEventData, CancellationToken.None), Times.Once);
        map.Verify(m => m(eventData), Times.Once);
    }
}
