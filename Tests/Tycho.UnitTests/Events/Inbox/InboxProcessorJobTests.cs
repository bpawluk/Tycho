using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Events;
using Tycho.Events.Inbox;
using Tycho.Events.Model;
using Tycho.Events.Registrating.Registrations;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.Structure;
using Tycho.Transactions;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Events.Inbox;

public class InboxProcessorJobTests
{
    private readonly Mock<IInboxConsumer> _inboxConsumerMock;
    private readonly Mock<ITransaction> _transactionMock;
    private readonly Mock<IEventHandler<TestEvent>> _handlerMock;
    private readonly Mock<ITransactionalEventHandler<TestEvent>> _transactionalHandlerMock;
    private readonly Mock<IFinalEventRegistration<TestEvent>> _registrationMock;

    public InboxProcessorJobTests()
    {
        _inboxConsumerMock = new Mock<IInboxConsumer>();
        _inboxConsumerMock.Setup(i => i.MarkAsHandled(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(true);
        _inboxConsumerMock.Setup(i => i.MarkAsFailed(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(true);

        _transactionMock = new Mock<ITransaction>();
        _transactionMock.Setup(t => t.BeginAsync(It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);
        _transactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);
        _transactionMock.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

        _handlerMock = new Mock<IEventHandler<TestEvent>>();
        _handlerMock.Setup(h => h.HandleAsync(It.IsAny<EventContext<TestEvent>>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

        _transactionalHandlerMock = new Mock<ITransactionalEventHandler<TestEvent>>();
        _transactionalHandlerMock.Setup(h => h.HandleAsync(It.IsAny<EventContext<TestEvent>>(), It.IsAny<CancellationToken>()))
                                 .Returns(Task.CompletedTask);

        _registrationMock = new Mock<IFinalEventRegistration<TestEvent>>();
        _registrationMock.SetupGet(r => r.HandlerId)
                         .Returns(EventHandlerIdentity.Create<TestEventHandler>());
    }

    [Fact]
    public async Task ExecuteAsync_WithNoEventAssigned_ReturnsEarly()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        InboxProcessorJob sut = CreateSut();

        // Act
        await sut.ExecuteAsync(cancellationToken);

        // Assert
        _registrationMock.VerifyGet(r => r.Handler, Times.Never);
        _transactionMock.Verify(t => t.BeginAsync(cancellationToken), Times.Never);
        _handlerMock.Verify(h => h.HandleAsync(It.IsAny<EventContext<TestEvent>>(), cancellationToken), Times.Never);
        _inboxConsumerMock.Verify(i => i.MarkAsHandled(It.IsAny<Guid>(), It.IsAny<Guid>(), cancellationToken), Times.Never);
        _transactionMock.Verify(t => t.CommitAsync(cancellationToken), Times.Never);
        _transactionMock.Verify(t => t.RollbackAsync(cancellationToken), Times.Never);
        _inboxConsumerMock.Verify(i => i.MarkAsFailed(It.IsAny<Guid>(), It.IsAny<Guid>(), cancellationToken), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithAssignedEvent_HandlesEventAndMarksAsHandled()
    {
        // Arrange
        InboxEvent inboxEvent = CreateInboxEvent(out RoutedEvent<TestEvent> routedEvent);
        var cancellationToken = new CancellationToken();
        InboxProcessorJob sut = CreateSut();

        // Act
        sut.ForEvent(inboxEvent);
        await sut.ExecuteAsync(cancellationToken);

        // Assert
        _registrationMock.Verify(r => r.Handler, Times.Once);
        _transactionMock.Verify(t => t.BeginAsync(cancellationToken), Times.Never);
        _handlerMock.Verify(
            h => h.HandleAsync(
                It.Is<EventContext<TestEvent>>(c => c.Id == routedEvent.Id && c.Payload == routedEvent.Payload),
                cancellationToken),
            Times.Once);
        _inboxConsumerMock.Verify(i => i.MarkAsHandled(inboxEvent.EventId, inboxEvent.ClaimId, cancellationToken), Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(cancellationToken), Times.Never);
        _transactionMock.Verify(t => t.RollbackAsync(cancellationToken), Times.Never);
        _inboxConsumerMock.Verify(i => i.MarkAsFailed(inboxEvent.EventId, inboxEvent.ClaimId, cancellationToken), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithAssignedEvent_AndTransactionalHandler_HandlesEventAndMarksAsHandledWithinTransaction()
    {
        // Arrange
        InboxEvent inboxEvent = CreateInboxEvent(out RoutedEvent<TestEvent> routedEvent);
        var cancellationToken = new CancellationToken();
        InboxProcessorJob sut = CreateSut(useTransactionalHandler: true);

        // Act
        sut.ForEvent(inboxEvent);
        await sut.ExecuteAsync(cancellationToken);

        // Assert
        _registrationMock.Verify(r => r.Handler, Times.Once);
        _transactionMock.Verify(t => t.BeginAsync(cancellationToken), Times.Once);
        _transactionalHandlerMock.Verify(
            h => h.HandleAsync(
                It.Is<EventContext<TestEvent>>(c => c.Id == routedEvent.Id && c.Payload == routedEvent.Payload),
                cancellationToken),
            Times.Once);
        _inboxConsumerMock.Verify(i => i.MarkAsHandled(inboxEvent.EventId, inboxEvent.ClaimId, cancellationToken), Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(cancellationToken), Times.Once);
        _transactionMock.Verify(t => t.RollbackAsync(cancellationToken), Times.Never);
        _inboxConsumerMock.Verify(i => i.MarkAsFailed(inboxEvent.EventId, inboxEvent.ClaimId, cancellationToken), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithAssignedEvent_WhenHandlerThrows_MarksEventAsFailed()
    {
        // Arrange
        InboxEvent inboxEvent = CreateInboxEvent(out _);
        var cancellationToken = new CancellationToken();
        InboxProcessorJob sut = CreateSut();

        _handlerMock.Setup(h => h.HandleAsync(It.IsAny<EventContext<TestEvent>>(), cancellationToken))
                    .ThrowsAsync(new InvalidOperationException("handler failure"));

        // Act
        sut.ForEvent(inboxEvent);
        await sut.ExecuteAsync(cancellationToken);

        // Assert
        _registrationMock.Verify(r => r.Handler, Times.Once);
        _transactionMock.Verify(t => t.BeginAsync(cancellationToken), Times.Never);
        _inboxConsumerMock.Verify(i => i.MarkAsHandled(inboxEvent.EventId, inboxEvent.ClaimId, cancellationToken), Times.Never);
        _transactionMock.Verify(t => t.CommitAsync(cancellationToken), Times.Never);
        _transactionMock.Verify(t => t.RollbackAsync(cancellationToken), Times.Never);
        _inboxConsumerMock.Verify(i => i.MarkAsFailed(inboxEvent.EventId, inboxEvent.ClaimId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithAssignedEvent_WhenTransactionalHandlerThrows_RollbacksAndMarksEventAsFailed()
    {
        // Arrange
        InboxEvent inboxEvent = CreateInboxEvent(out _);
        var cancellationToken = new CancellationToken();
        InboxProcessorJob sut = CreateSut(useTransactionalHandler: true);

        _transactionalHandlerMock.Setup(h => h.HandleAsync(It.IsAny<EventContext<TestEvent>>(), cancellationToken))
                                 .ThrowsAsync(new InvalidOperationException("handler failure"));

        // Act
        sut.ForEvent(inboxEvent);
        await sut.ExecuteAsync(cancellationToken);

        // Assert
        _registrationMock.Verify(r => r.Handler, Times.Once);
        _transactionMock.Verify(t => t.BeginAsync(cancellationToken), Times.Once);
        _inboxConsumerMock.Verify(i => i.MarkAsHandled(inboxEvent.EventId, inboxEvent.ClaimId, cancellationToken), Times.Never);
        _transactionMock.Verify(t => t.CommitAsync(cancellationToken), Times.Never);
        _transactionMock.Verify(t => t.RollbackAsync(cancellationToken), Times.Once);
        _inboxConsumerMock.Verify(i => i.MarkAsFailed(inboxEvent.EventId, inboxEvent.ClaimId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithAssignedEvent_WhenHandlerNotFound_MarksEventAsFailed()
    {
        // Arrange
        InboxEvent inboxEvent = CreateInboxEvent(out _);
        var cancellationToken = new CancellationToken();
        InboxProcessorJob sut = CreateSut(withHandler: false);

        // Act
        sut.ForEvent(inboxEvent);
        await sut.ExecuteAsync(cancellationToken);

        // Assert
        _inboxConsumerMock.Verify(i => i.MarkAsHandled(inboxEvent.EventId, inboxEvent.ClaimId, cancellationToken), Times.Never);
        _inboxConsumerMock.Verify(i => i.MarkAsFailed(inboxEvent.EventId, inboxEvent.ClaimId, cancellationToken), Times.Once);
    }

    private InboxProcessorJob CreateSut(bool withHandler = true, bool useTransactionalHandler = false)
    {
        var internals = new Internals(typeof(TestModule));
        IServiceCollection serviceCollection = internals.GetServiceCollection();

        serviceCollection.AddSingleton(_inboxConsumerMock.Object);
        serviceCollection.AddSingleton(_transactionMock.Object);
        if (withHandler)
        {
            if (useTransactionalHandler)
            {
                _registrationMock.SetupGet(r => r.Handler)
                                 .Returns(_transactionalHandlerMock.Object);
                _transactionMock.SetupGet(t => t.IsInProgress)
                                .Returns(true);
            }
            else
            {
                _registrationMock.SetupGet(r => r.Handler)
                                 .Returns(_handlerMock.Object);
                _transactionMock.SetupGet(t => t.IsInProgress)
                                .Returns(false);
            }
            serviceCollection.AddSingleton(_registrationMock.Object);
        }

        internals.Build();
        return new InboxProcessorJob(internals);
    }

    private static InboxEvent CreateInboxEvent(out RoutedEvent<TestEvent> routedEvent)
    {
        var eventId = EventIdentity.Create<TestEvent>();
        var handlerId = EventHandlerIdentity.Create<TestEventHandler>();
        routedEvent = new RoutedEvent<TestEvent>(Guid.NewGuid(), Guid.NewGuid(), eventId, handlerId, Route.Create(), new TestEvent());
        return new InboxEvent(Guid.NewGuid(), routedEvent);
    }
}
