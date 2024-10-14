using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Sources;
using Tycho.Structure;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Events.Routing.Sources;

public class LocalHandlersSourceTests
{
    private readonly LocalHandlersSource<TestEvent> _sut;

    public LocalHandlersSourceTests()
    {
        var internals = new Internals(typeof(TestModule));
        var services = internals.GetServiceCollection();
        services.AddTransient<IEventHandler<OtherEvent>, OtherEventHandler>();
        services.AddTransient<IEventHandler<TestEvent>, TestEventOtherHandler>();
        services.AddTransient<IEventHandler<TestEvent>, TestEventHandler>();
        internals.Build();
        _sut = new LocalHandlersSource<TestEvent>(internals);
    }

    [Fact]
    public void IdentifyHandlers_ForTestEvent_ReturnsRegisteredHandlers()
    {
        // Arrange
        var expectedIdentities = new[]
        {
            new HandlerIdentity(typeof(TestEvent), typeof(TestEventOtherHandler), typeof(TestModule)),
            new HandlerIdentity(typeof(TestEvent), typeof(TestEventHandler), typeof(TestModule))
        };

        // Act
        var result = _sut.IdentifyHandlers<TestEvent>();

        // Assert
        Assert.Equal(expectedIdentities, result);
    }

    [Fact]
    public void IdentifyHandlers_ForOtherEvent_ReturnsEmptyCollection()
    {
        // Arrange
        // - no arrangement required

        // Act
        var result = _sut.IdentifyHandlers<OtherEvent>();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void FindHandler_ForIdentityOfTestEventHandlerAndTestModule_ReturnsHandlerMatchingTheIdentity()
    {
        // Arrange
        var identity = new HandlerIdentity(typeof(TestEvent), typeof(TestEventHandler), typeof(TestModule));

        // Act
        var result = _sut.FindHandler(identity);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<TestEventHandler>(result);
    }

    [Fact]
    public void FindHandler_ForIdentityOfTestEventHandlerAndOtherModule_ReturnsNull()
    {
        // Arrange
        var identity = new HandlerIdentity(typeof(TestEvent), typeof(TestEventHandler), typeof(OtherModule));

        // Act
        var result = _sut.FindHandler(identity);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FindHandler_ForIdentityOfOtherEventHandlerAndTestModule_ReturnsNull()
    {
        // Arrange
        var identity = new HandlerIdentity(typeof(OtherEvent), typeof(TestEventHandler), typeof(TestModule));

        // Act
        var result = _sut.FindHandler(identity);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FindHandler_WithMissingHandlerRegistration_ThrowsInvalidOperationException()
    {
        // Arrange
        var identity = new HandlerIdentity(typeof(TestEvent), typeof(TestEventAnotherHandler), typeof(TestModule));

        // Act
        var getResult = () => _sut.FindHandler(identity);

        // Assert
        Assert.Throws<InvalidOperationException>(getResult);
    }
}
