using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        _internals = new Internals(typeof(object), Host.CreateEmptyApplicationBuilder(default));
        _sut = new AppEvents(_internals);
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
    public void Build_WhenCustomInboxAlreadyRegistered_DoesNotOverrideIt()
    {
        // Arrange
        var customInbox = new Mock<IInboxWriter>();
        _internals.GetHostBuilder().Services.AddSingleton(customInbox.Object);
        _internals.GetHostBuilder().Services.AddSingleton(Mock.Of<IInboxConsumer>());

        // Act
        _sut.Build();

        // Assert
        Assert.True(_internals.HasService<IInboxWriter>());
        var descriptors = _internals.GetHostBuilder().Services
            .Where(d => d.ServiceType == typeof(IInboxWriter))
            .ToList();
        Assert.Single(descriptors);
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
    public void Build_WhenCustomOutboxAlreadyRegistered_DoesNotOverrideIt()
    {
        // Arrange
        var customOutbox = new Mock<IOutboxWriter>();
        _internals.GetHostBuilder().Services.AddSingleton(customOutbox.Object);
        _internals.GetHostBuilder().Services.AddSingleton(Mock.Of<IOutboxConsumer>());

        // Act
        _sut.Build();

        // Assert
        Assert.True(_internals.HasService<IOutboxWriter>());
        var descriptors = _internals.GetHostBuilder().Services
            .Where(d => d.ServiceType == typeof(IOutboxWriter))
            .ToList();
        Assert.Single(descriptors);
    }
}
