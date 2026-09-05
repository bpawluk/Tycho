using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        _internals = new Internals(typeof(object), Host.CreateEmptyApplicationBuilder(default));
        _sut = new ModuleEvents(_internals);
    }

    [Fact]
    public void Build_WhenNoCustomInbox_RegistersInMemoryInbox()
    {
        // Act
        _sut.Build();

        // Assert
        Assert.True(_internals.HasService<IInboxWriter>());
        Assert.True(_internals.HasService<IInboxConsumer>());
    }

    [Fact]
    public void Build_WhenCustomInboxAlreadyRegistered_DoesNotRegisterInMemoryInbox()
    {
        // Arrange
        _internals.GetHostBuilder().Services.AddSingleton(Mock.Of<IInboxWriter>());
        _internals.GetHostBuilder().Services.AddSingleton(Mock.Of<IInboxConsumer>());

        // Act
        _sut.Build();

        // Assert
        var inboxWriterDescriptors = _internals.GetHostBuilder().Services
            .Where(d => d.ServiceType == typeof(IInboxWriter))
            .ToList();
        Assert.Single(inboxWriterDescriptors);
    }

    [Fact]
    public void Build_WhenNoCustomOutbox_RegistersInMemoryOutbox()
    {
        // Act
        _sut.Build();

        // Assert
        Assert.True(_internals.HasService<IOutboxWriter>());
        Assert.True(_internals.HasService<IOutboxConsumer>());
    }

    [Fact]
    public void Build_WhenCustomOutboxAlreadyRegistered_DoesNotRegisterInMemoryOutbox()
    {
        // Arrange
        _internals.GetHostBuilder().Services.AddSingleton(Mock.Of<IOutboxWriter>());
        _internals.GetHostBuilder().Services.AddSingleton(Mock.Of<IOutboxConsumer>());

        // Act
        _sut.Build();

        // Assert
        var outboxWriterDescriptors = _internals.GetHostBuilder().Services
            .Where(d => d.ServiceType == typeof(IOutboxWriter))
            .ToList();
        Assert.Single(outboxWriterDescriptors);
    }

}
