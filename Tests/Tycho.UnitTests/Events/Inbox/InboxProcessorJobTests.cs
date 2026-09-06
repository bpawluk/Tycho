using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
    private readonly Mock<IInboxConsumer> _inboxConsumerMock = new();
    private readonly Mock<ITransaction> _transactionMock = new();
    private readonly Mock<IEventHandler<TestEvent>> _handlerMock = new();
    private readonly Mock<ITransactionalEventHandler<TestEvent>> _transactionalHandlerMock = new();
    private readonly Mock<IFinalEventRegistration<TestEvent>> _registrationMock = new();

    public InboxProcessorJobTests()
    {
        _inboxConsumerMock
            .Setup(inbox => inbox.MarkAsHandledAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _inboxConsumerMock
            .Setup(inbox => inbox.MarkAsFailedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _transactionMock
            .Setup(transaction => transaction.ExecuteAsync(
                It.IsAny<Func<CancellationToken, Task>>(),
                It.IsAny<CancellationToken>()))
            .Returns((Func<CancellationToken, Task> operation, CancellationToken token) => operation(token));
        _handlerMock
            .Setup(handler => handler.HandleAsync(
                It.IsAny<EventContext<TestEvent>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _transactionalHandlerMock
            .Setup(handler => handler.HandleAsync(
                It.IsAny<EventContext<TestEvent>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _registrationMock.SetupGet(registration => registration.HandlerId)
            .Returns(EventHandlerIdentity.Create<TestEventHandler>());
    }

    [Fact]
    public async Task ExecuteAsync_WithNoEventAssigned_ReturnsEarly()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        await CreateSut().ExecuteAsync(cancellationToken);

        _registrationMock.VerifyGet(registration => registration.Handler, Times.Never);
        VerifyTransactionExecution(cancellationToken, Times.Never());
        _inboxConsumerMock.Verify(inbox => inbox.MarkAsHandledAsync(
            It.IsAny<Guid>(), cancellationToken), Times.Never);
        _inboxConsumerMock.Verify(inbox => inbox.MarkAsFailedAsync(
            It.IsAny<Guid>(), cancellationToken), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonTransactionalHandler_HandlesAndAcknowledgesWithoutTransaction()
    {
        InboxEvent inboxEvent = CreateInboxEvent(out RoutedEvent<TestEvent> routedEvent);
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        InboxProcessorJob sut = CreateSut();

        await sut.ForEvent(inboxEvent).ExecuteAsync(cancellationToken);

        _handlerMock.Verify(handler => handler.HandleAsync(
            It.Is<EventContext<TestEvent>>(context =>
                context.Id == routedEvent.Id && context.Payload == routedEvent.Payload),
            cancellationToken), Times.Once);
        _inboxConsumerMock.Verify(inbox => inbox.MarkAsHandledAsync(
            inboxEvent.ClaimId, cancellationToken), Times.Once);
        VerifyTransactionExecution(cancellationToken, Times.Never());
        _inboxConsumerMock.Verify(inbox => inbox.MarkAsFailedAsync(
            inboxEvent.ClaimId, cancellationToken), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithTransactionalHandler_HandlesAndAcknowledgesInsideTransaction()
    {
        InboxEvent inboxEvent = CreateInboxEvent(out _);
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        InboxProcessorJob sut = CreateSut(useTransactionalHandler: true);

        await sut.ForEvent(inboxEvent).ExecuteAsync(cancellationToken);

        _transactionalHandlerMock.Verify(handler => handler.HandleAsync(
            It.IsAny<EventContext<TestEvent>>(), cancellationToken), Times.Once);
        _inboxConsumerMock.Verify(inbox => inbox.MarkAsHandledAsync(
            inboxEvent.ClaimId, cancellationToken), Times.Once);
        VerifyTransactionExecution(cancellationToken, Times.Once());
        _inboxConsumerMock.Verify(inbox => inbox.MarkAsFailedAsync(
            inboxEvent.ClaimId, cancellationToken), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTransactionalHandlerFails_MarksClaimAsFailed()
    {
        InboxEvent inboxEvent = CreateInboxEvent(out _);
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        _transactionalHandlerMock
            .Setup(handler => handler.HandleAsync(
                It.IsAny<EventContext<TestEvent>>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("handler failure"));
        InboxProcessorJob sut = CreateSut(useTransactionalHandler: true);

        await sut.ForEvent(inboxEvent).ExecuteAsync(cancellationToken);

        VerifyTransactionExecution(cancellationToken, Times.Once());
        _inboxConsumerMock.Verify(inbox => inbox.MarkAsHandledAsync(
            inboxEvent.ClaimId, cancellationToken), Times.Never);
        _inboxConsumerMock.Verify(inbox => inbox.MarkAsFailedAsync(
            inboxEvent.ClaimId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTransactionalHandlerLosesClaim_FailsTransactionAndAttemptsFailureMark()
    {
        InboxEvent inboxEvent = CreateInboxEvent(out _);
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        _inboxConsumerMock
            .Setup(inbox => inbox.MarkAsHandledAsync(inboxEvent.ClaimId, cancellationToken))
            .ReturnsAsync(false);
        _inboxConsumerMock
            .Setup(inbox => inbox.MarkAsFailedAsync(inboxEvent.ClaimId, cancellationToken))
            .ReturnsAsync(false);
        InboxProcessorJob sut = CreateSut(useTransactionalHandler: true);

        await sut.ForEvent(inboxEvent).ExecuteAsync(cancellationToken);

        VerifyTransactionExecution(cancellationToken, Times.Once());
        _inboxConsumerMock.Verify(inbox => inbox.MarkAsHandledAsync(
            inboxEvent.ClaimId, cancellationToken), Times.Once);
        _inboxConsumerMock.Verify(inbox => inbox.MarkAsFailedAsync(
            inboxEvent.ClaimId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenHandlerCannotBeResolved_MarksClaimAsFailed()
    {
        InboxEvent inboxEvent = CreateInboxEvent(out _);
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        InboxProcessorJob sut = CreateSut(withHandler: false);

        await sut.ForEvent(inboxEvent).ExecuteAsync(cancellationToken);

        _inboxConsumerMock.Verify(inbox => inbox.MarkAsHandledAsync(
            inboxEvent.ClaimId, cancellationToken), Times.Never);
        _inboxConsumerMock.Verify(inbox => inbox.MarkAsFailedAsync(
            inboxEvent.ClaimId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTransactionalProcessingFails_DisposesScopeBeforeFailureBookkeeping()
    {
        var calls = new List<string>();
        InboxEvent inboxEvent = CreateInboxEvent(out _);
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        var internals = new Internals(typeof(TestModule), Host.CreateEmptyApplicationBuilder(default));
        IServiceCollection services = internals.GetHostBuilder().Services;
        int inboxInstance = 0;

        services.AddScoped<IInboxConsumer>(_ =>
        {
            int instance = ++inboxInstance;
            calls.Add($"inbox-{instance}-created");
            var inbox = new Mock<IInboxConsumer>();
            inbox.Setup(consumer => consumer.MarkAsFailedAsync(inboxEvent.ClaimId, cancellationToken))
                .Callback(() => calls.Add("claim-failed"))
                .ReturnsAsync(true);
            inbox.As<IAsyncDisposable>().Setup(value => value.DisposeAsync())
                .Callback(() => calls.Add($"inbox-{instance}-disposed"))
                .Returns(default(ValueTask));
            return inbox.Object;
        });

        var transaction = new Mock<ITransaction>();
        transaction.Setup(value => value.ExecuteAsync(
                It.IsAny<Func<CancellationToken, Task>>(), cancellationToken))
            .Returns((Func<CancellationToken, Task> operation, CancellationToken token) => operation(token));
        services.AddScoped(_ => transaction.Object);

        var handler = new Mock<ITransactionalEventHandler<TestEvent>>();
        handler.Setup(value => value.HandleAsync(
                It.IsAny<EventContext<TestEvent>>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("handler failure"));
        _registrationMock.SetupGet(registration => registration.Handler).Returns(handler.Object);
        services.AddSingleton(_registrationMock.Object);
        internals.Build();

        var sut = new InboxProcessorJob(internals).ForEvent(inboxEvent);

        await sut.ExecuteAsync(cancellationToken);

        Assert.Equal(
            ["inbox-1-created", "inbox-1-disposed", "inbox-2-created", "claim-failed", "inbox-2-disposed"],
            calls);
    }

    private InboxProcessorJob CreateSut(bool withHandler = true, bool useTransactionalHandler = false)
    {
        var internals = new Internals(typeof(TestModule), Host.CreateEmptyApplicationBuilder(default));
        IServiceCollection services = internals.GetHostBuilder().Services;
        services.AddSingleton(_inboxConsumerMock.Object);
        services.AddSingleton(_transactionMock.Object);

        if (withHandler)
        {
            _registrationMock.SetupGet(registration => registration.Handler)
                .Returns(useTransactionalHandler
                    ? _transactionalHandlerMock.Object
                    : _handlerMock.Object);
            services.AddSingleton(_registrationMock.Object);
        }

        internals.Build();
        return new InboxProcessorJob(internals);
    }

    private void VerifyTransactionExecution(CancellationToken cancellationToken, Times times)
    {
        _transactionMock.Verify(transaction => transaction.ExecuteAsync(
            It.IsAny<Func<CancellationToken, Task>>(), cancellationToken), times);
    }

    private static InboxEvent CreateInboxEvent(out RoutedEvent<TestEvent> routedEvent)
    {
        routedEvent = new RoutedEvent<TestEvent>(
            Guid.NewGuid(),
            Guid.NewGuid(),
            EventIdentity.Create<TestEvent>(),
            EventHandlerIdentity.Create<TestEventHandler>(),
            Route.Create(),
            new TestEvent());
        return new InboxEvent(Guid.NewGuid(), routedEvent);
    }
}
