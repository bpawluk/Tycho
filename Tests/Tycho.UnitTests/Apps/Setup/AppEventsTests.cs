using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Apps.Setup;
using Tycho.Events.Inbox;
using Tycho.Events.Outbox;
using Tycho.Structure;

namespace Tycho.UnitTests.Apps.Setup;

public class AppEventsTests
{
    private readonly Internals _internals;
    private readonly AppEvents _sut;

    public AppEventsTests()
    {
        _internals = new Internals(typeof(object));
        _sut = new AppEvents(_internals);
    }

    [Fact]
    public async Task BuildAsync_WhenNoCustomInbox_RegistersInMemoryInbox()
    {
        // Act
        await _sut.BuildAsync();

        // Assert
        Assert.True(_internals.HasService<IInboxWriter>());
        Assert.True(_internals.HasService<IInboxConsumer>());
    }

    [Fact]
    public async Task BuildAsync_WhenCustomInboxAlreadyRegistered_DoesNotOverrideIt()
    {
        // Arrange
        var customInbox = new Mock<IInboxWriter>();
        _internals.GetServiceCollection().AddSingleton(customInbox.Object);
        _internals.GetServiceCollection().AddSingleton(Mock.Of<IInboxConsumer>());

        // Act
        await _sut.BuildAsync();

        // Assert
        Assert.True(_internals.HasService<IInboxWriter>());
        var descriptors = _internals.GetServiceCollection()
            .Where(d => d.ServiceType == typeof(IInboxWriter))
            .ToList();
        Assert.Single(descriptors);
    }

    [Fact]
    public async Task BuildAsync_WhenNoCustomOutbox_RegistersInMemoryOutbox()
    {
        // Act
        await _sut.BuildAsync();

        // Assert
        Assert.True(_internals.HasService<IOutboxWriter>());
        Assert.True(_internals.HasService<IOutboxConsumer>());
    }

    [Fact]
    public async Task BuildAsync_WhenCustomOutboxAlreadyRegistered_DoesNotOverrideIt()
    {
        // Arrange
        var customOutbox = new Mock<IOutboxWriter>();
        _internals.GetServiceCollection().AddSingleton(customOutbox.Object);
        _internals.GetServiceCollection().AddSingleton(Mock.Of<IOutboxConsumer>());

        // Act
        await _sut.BuildAsync();

        // Assert
        Assert.True(_internals.HasService<IOutboxWriter>());
        var descriptors = _internals.GetServiceCollection()
            .Where(d => d.ServiceType == typeof(IOutboxWriter))
            .ToList();
        Assert.Single(descriptors);
    }
}
