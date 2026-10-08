using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Tycho.Events;
using Tycho.Events.Broker;
using Tycho.Events.Model;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Steps;
using Tycho.Identity.Events;
using Tycho.Identity.Structure;
using Tycho.Modules;
using Tycho.Modules.Instance;
using Tycho.Modules.Setup;
using Tycho.Structure;
using Tycho.Structure.Parent;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Modules.Setup;

public sealed class ModuleEventsBindingTests : IDisposable
{
    private readonly Internals _internals = new(Host.CreateEmptyApplicationBuilder(null), typeof(TestModule));
    private readonly Mock<IEventBroker> _child = new(MockBehavior.Strict);
    private readonly Mock<IEventBroker> _parent = new(MockBehavior.Strict);
    private readonly Mock<IEventBroker> _unrelated = new(MockBehavior.Strict);
    private readonly ModuleEvents _sut;

    public ModuleEventsBindingTests()
    {
        _internals.GetHostBuilder().Services
            .AddSingleton(_internals)
            .AddSingleton(Mock.Of<IModule<OtherModule>>(module => module.EventBroker == _child.Object))
            .AddSingleton(Mock.Of<IModule<AnotherModule>>(module => module.EventBroker == _unrelated.Object))
            .AddSingleton(Mock.Of<IParentReference>(parent => parent.EventBroker == _parent.Object));
        _sut = new ModuleEvents(_internals);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task MappedBinding_WhenRouting_MapsPayloadAndCallsOnlySelectedDestination(bool expose)
    {
        // Arrange
        Guid publishId = Guid.NewGuid();
        CancellationToken token = TestContext.Current.CancellationToken;
        var source = new SourceEvent("input");
        var mapped = new TargetEvent("mapped:input");
        var routedEvent = new RoutedEvent<TargetEvent>(
            Guid.NewGuid(), publishId, EventIdentity.Create<TargetEvent>(),
            EventHandlerIdentity.Parse("target-handler"), Route.Create(), mapped);
        Mock<IEventBroker> destination = expose ? _parent : _child;
        destination.Setup(broker => broker.RouteAsync(publishId, mapped, token)).ReturnsAsync([routedEvent]);
        int mappingCalls = 0;
        IModuleEventBindingWithMapping<SourceEvent, TargetEvent> binding = _sut.Expects<SourceEvent>().MapsTo<TargetEvent>(payload =>
        {
            mappingCalls++;
            return new TargetEvent("mapped:" + payload.Value);
        });
        IModuleEventBindingWithMapping<SourceEvent, TargetEvent> returnedBinding = expose ? binding.Exposes() : binding.ForwardsTo<OtherModule>();
        _sut.Build();
        _internals.Build();
        await using AsyncServiceScope scope = _internals.CreateAsyncScope();
        IEventBroker broker = scope.ServiceProvider.GetRequiredService<IEventBroker>();

        // Act
        IReadOnlyCollection<RoutedEvent> result = await broker.RouteAsync(publishId, source, token);

        // Assert
        Assert.Same(binding, returnedBinding);
        Assert.Same(routedEvent, Assert.Single(result));
        Assert.Equal(1, mappingCalls);
        IRouteStep[] steps = [.. routedEvent.Route];
        Assert.Equal(2, steps.Length);
        if (expose)
        {
            Assert.IsType<UpStreamRouteStep>(steps[0]);
        }
        else
        {
            DownStreamRouteStep step = Assert.IsType<DownStreamRouteStep>(steps[0]);
            Assert.Equal(DefinitionIdentity.Create<OtherModule>(), step.Destination);
        }
        Assert.IsType<FinalRouteStep>(steps[1]);
        destination.Verify(target => target.RouteAsync(publishId, mapped, token), Times.Once);
        _child.VerifyNoOtherCalls();
        _parent.VerifyNoOtherCalls();
        _unrelated.VerifyNoOtherCalls();
    }

    public void Dispose() => _internals.Dispose();

    public sealed record SourceEvent(string Value) : IEvent;
    public sealed record TargetEvent(string Value) : IEvent;
}
