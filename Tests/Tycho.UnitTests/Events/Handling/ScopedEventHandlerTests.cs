using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Events;
using Tycho.Events.Handling;
using Tycho.Events.Inbox;
using Tycho.Structure;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Events.Handling;

public class ScopedEventHandlerTests
{
    private readonly Mock<IInboxConsumer> _inboxConsumerMock = new();

    public ScopedEventHandlerTests()
    {
        _inboxConsumerMock.Setup(ic => ic.MarkAsHandled(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                          .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task HandleAsync_WithRegularHandler_HandlesTheEvent()
    {
        // Arrange
        var context = CreateEventContext();
        var cancellationToken = new CancellationToken();

        var handlerMock = new Mock<IEventHandler<TestEvent>>();
        var internals = CreateInternals(handlerMock.Object);
        var sut = new ScopedEventHandler<TestEvent, IEventHandler<TestEvent>>(internals);

        // Act
        await sut.HandleAsync(context, cancellationToken);

        // Assert
        handlerMock.Verify(eh => eh.HandleAsync(context, cancellationToken));
        _inboxConsumerMock.Verify(ic => ic.MarkAsHandled(context.Id, cancellationToken));
    }

    [Fact]
    public async Task HandleAsync_WithRegularHandler_AndTheHandlerThrowingException_RethrowsTheException()
    {
        // Arrange
        var context = CreateEventContext();
        var cancellationToken = new CancellationToken();

        var handlerMock = new Mock<IEventHandler<TestEvent>>();
        handlerMock.Setup(eh => eh.HandleAsync(context, cancellationToken))
                   .ThrowsAsync(new InvalidOperationException());

        var internals = CreateInternals(handlerMock.Object);
        var sut = new ScopedEventHandler<TestEvent, IEventHandler<TestEvent>>(internals);

        // Act
        Task Act() => sut.HandleAsync(context, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
        _inboxConsumerMock.Verify(ic => ic.MarkAsHandled(context.Id, cancellationToken), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithTransactionalHandler_HandlesTheEventWithinTransaction()
    {
        // Arrange
        var context = CreateEventContext();
        var cancellationToken = new CancellationToken();

        var sequence = 0;
        var transactionalHandlerMock = new Mock<ITransactionalEventHandler<TestEvent>>();

        transactionalHandlerMock.When(() => sequence == 0)
                                .Setup(th => th.BeginTransactionAsync(cancellationToken))
                                .Callback(() => sequence++)
                                .Returns(Task.CompletedTask);

        transactionalHandlerMock.When(() => sequence != 0)
                                .Setup(th => th.BeginTransactionAsync(cancellationToken))
                                .Throws(new InvalidOperationException());

        transactionalHandlerMock.When(() => sequence == 1)
                                .Setup(th => th.HandleAsync(context, cancellationToken))
                                .Callback(() => sequence++)
                                .Returns(Task.CompletedTask);

        transactionalHandlerMock.When(() => sequence != 1)
                                .Setup(th => th.HandleAsync(context, cancellationToken))
                                .Throws(new InvalidOperationException());

        _inboxConsumerMock.When(() => sequence == 2)
                          .Setup(ic => ic.MarkAsHandled(context.Id, cancellationToken))
                          .Callback(() => sequence++)
                          .Returns(Task.CompletedTask);

        _inboxConsumerMock.When(() => sequence != 2)
                          .Setup(ic => ic.MarkAsHandled(context.Id, cancellationToken))
                          .Throws(new InvalidOperationException());

        transactionalHandlerMock.When(() => sequence == 3)
                                .Setup(th => th.CommitTransactionAsync(cancellationToken))
                                .Callback(() => sequence++)
                                .Returns(Task.CompletedTask);

        transactionalHandlerMock.When(() => sequence != 3)
                                .Setup(th => th.CommitTransactionAsync(cancellationToken))
                                .Throws(new InvalidOperationException());

        var internals = CreateInternals(transactionalHandlerMock.Object);
        var sut = new ScopedEventHandler<TestEvent, ITransactionalEventHandler<TestEvent>>(internals);

        // Act
        await sut.HandleAsync(context, cancellationToken);

        // Assert
        transactionalHandlerMock.Verify(th => th.BeginTransactionAsync(cancellationToken), Times.Once);
        transactionalHandlerMock.Verify(th => th.HandleAsync(context, cancellationToken), Times.Once);
        _inboxConsumerMock.Verify(ic => ic.MarkAsHandled(context.Id, cancellationToken), Times.Once);
        transactionalHandlerMock.Verify(th => th.CommitTransactionAsync(cancellationToken), Times.Once);
        transactionalHandlerMock.Verify(th => th.RollbackTransactionAsync(cancellationToken), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithTransactionalHandler_AndTheHandlerThrowingException_RollsBackTheTransaction()
    {
        // Arrange
        var context = CreateEventContext();
        var cancellationToken = new CancellationToken();

        var transactionalHandlerMock = new Mock<ITransactionalEventHandler<TestEvent>>();
        transactionalHandlerMock.Setup(th => th.HandleAsync(context, cancellationToken))
                                .ThrowsAsync(new InvalidOperationException());

        var internals = CreateInternals(transactionalHandlerMock.Object);
        var sut = new ScopedEventHandler<TestEvent, ITransactionalEventHandler<TestEvent>>(internals);

        // Act
        Task Act() => sut.HandleAsync(context, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
        transactionalHandlerMock.Verify(th => th.BeginTransactionAsync(cancellationToken), Times.Once);
        transactionalHandlerMock.Verify(th => th.HandleAsync(context, cancellationToken), Times.Once);
        _inboxConsumerMock.Verify(ic => ic.MarkAsHandled(context.Id, cancellationToken), Times.Never);
        transactionalHandlerMock.Verify(th => th.CommitTransactionAsync(cancellationToken), Times.Never);
        transactionalHandlerMock.Verify(th => th.RollbackTransactionAsync(cancellationToken), Times.Once);
    }

    private Internals CreateInternals<THandler>(THandler handler)
        where THandler : class, IEventHandler<TestEvent>
    {
        var internals = new Internals(typeof(TestModule));
        var serviceCollection = internals.GetServiceCollection();
        serviceCollection.AddSingleton(_inboxConsumerMock.Object);
        serviceCollection.AddSingleton(handler);
        internals.Build();
        return internals;
    }

    private static EventContext<TestEvent> CreateEventContext()
    {
        return new EventContext<TestEvent>(Guid.NewGuid(), new TestEvent());
    }
}
