using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Events.Inbox;
using Tycho.Events.Outbox;
using Tycho.Modules.Setup;
using Tycho.Structure;

namespace Tycho.UnitTests.Modules.Setup;

public class ModuleEventsTests
{
    private readonly Internals _internals;
    private readonly ModuleEvents _sut;

    public ModuleEventsTests()
    {
        _internals = new Internals(typeof(object));
        _sut = new ModuleEvents(_internals);
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
    public async Task BuildAsync_WhenCustomInboxAlreadyRegistered_DoesNotRegisterInMemoryInbox()
    {
        // Arrange
        _internals.GetServiceCollection().AddSingleton(Mock.Of<IInboxWriter>());
        _internals.GetServiceCollection().AddSingleton(Mock.Of<IInboxConsumer>());

        // Act
        await _sut.BuildAsync();

        // Assert
        var inboxWriterDescriptors = _internals.GetServiceCollection()
            .Where(d => d.ServiceType == typeof(IInboxWriter))
            .ToList();
        Assert.Single(inboxWriterDescriptors);
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
    public async Task BuildAsync_WhenCustomOutboxAlreadyRegistered_DoesNotRegisterInMemoryOutbox()
    {
        // Arrange
        _internals.GetServiceCollection().AddSingleton(Mock.Of<IOutboxWriter>());
        _internals.GetServiceCollection().AddSingleton(Mock.Of<IOutboxConsumer>());

        // Act
        await _sut.BuildAsync();

        // Assert
        var outboxWriterDescriptors = _internals.GetServiceCollection()
            .Where(d => d.ServiceType == typeof(IOutboxWriter))
            .ToList();
        Assert.Single(outboxWriterDescriptors);
    }

}
