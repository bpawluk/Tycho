using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Tycho.Events.Delivery;
using Tycho.Events.Inbox;
using Tycho.Events.Model;
using Tycho.Identity.Events;
using Tycho.Identity.Structure;
using Tycho.Structure;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Delivery;

public sealed class DeliveryEndpointTests : IDisposable
{
    private readonly AppInternals _internals = new(
        Host.CreateEmptyApplicationBuilder(null),
        typeof(DeliveryEndpointTests));
    private readonly Mock<IInboxWriter> _inboxWriterMock = new(MockBehavior.Strict);
    private readonly DeliveryEndpoint _sut;

    public DeliveryEndpointTests()
    {
        _internals.GetHostBuilder().Services.AddScoped<IInboxWriter>(_ => _inboxWriterMock.Object);
        _internals.Build();
        _sut = new DeliveryEndpoint(_internals);
    }

    [Fact]
    public async Task AcceptAsync_WithMatchingDestination_WritesOriginalEventAndTokenToInbox()
    {
        // Arrange
        SerializedRoutedEvent routedEvent = CreateEvent(_internals.OwnerId);
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        _inboxWriterMock.Setup(inbox => inbox.Write(routedEvent, cancellationToken)).Returns(Task.CompletedTask);

        // Act
        await _sut.AcceptAsync(routedEvent, cancellationToken);

        // Assert
        _inboxWriterMock.Verify(inbox => inbox.Write(routedEvent, cancellationToken), Times.Once);
        _inboxWriterMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task AcceptAsync_WithDifferentDestination_ThrowsWithoutWritingToInbox()
    {
        // Arrange
        SerializedRoutedEvent routedEvent = CreateEvent(InstanceIdentity.Parse("another-module"));
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        // Act
        Task Act() => _sut.AcceptAsync(routedEvent, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);

        _inboxWriterMock.VerifyNoOtherCalls();
    }

    private static SerializedRoutedEvent CreateEvent(InstanceIdentity destination)
    {
        return new SerializedRoutedEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            EventIdentity.Create<TestEvent>(),
            EventHandlerIdentity.Create<TestEventHandler>(),
            destination,
            "{}");
    }

    public void Dispose() => _internals.Dispose();
}
