using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Tycho.Events;
using Tycho.Events.Model;
using Tycho.Events.Routing;
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
    private readonly Internals _internals = new ModuleInternals(
        Host.CreateEmptyApplicationBuilder(null),
        new ControlPlane(InstanceIdentity.Create(typeof(ModuleEventsBindingTests))),
        typeof(ModuleEventsBindingTests));
    private readonly Mock<IEventRouter> _child = new(MockBehavior.Strict);
    private readonly Mock<IEventRouter> _parent = new(MockBehavior.Strict);
    private readonly Mock<IEventRouter> _unrelated = new(MockBehavior.Strict);
    private readonly ModuleEvents _sut;

    public ModuleEventsBindingTests()
    {
        _internals.GetHostBuilder().Services
            .AddSingleton(_internals)
            .AddSingleton(Mock.Of<IModule<OtherModule>>(module => module.EventRouter == _child.Object))
            .AddSingleton(Mock.Of<IModule<AnotherModule>>(module => module.EventRouter == _unrelated.Object))
            .AddSingleton(Mock.Of<IParentReference>(parent => parent.EventRouter == _parent.Object));
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
            Guid.NewGuid(), publishId,
            EventIdentity.Create<TargetEvent>(),
            EventHandlerIdentity.Parse("target-handler"),
            InstanceIdentity.Parse("test-endpoint"),
            mapped);
        Mock<IEventRouter> destination = expose ? _parent : _child;
        destination.Setup(router => router.RouteAsync(publishId, mapped, token)).ReturnsAsync([routedEvent]);
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
        IEventRouter router = scope.ServiceProvider.GetRequiredService<IEventRouter>();

        // Act
        IReadOnlyCollection<RoutedEvent> result = await router.RouteAsync(publishId, source, token);

        // Assert
        Assert.Same(binding, returnedBinding);
        Assert.Same(routedEvent, Assert.Single(result));
        Assert.Equal(1, mappingCalls);
        Assert.Equal(InstanceIdentity.Parse("test-endpoint"), routedEvent.DestinationId);
        destination.Verify(target => target.RouteAsync(publishId, mapped, token), Times.Once);
        _child.VerifyNoOtherCalls();
        _parent.VerifyNoOtherCalls();
        _unrelated.VerifyNoOtherCalls();
    }

    public void Dispose() => _internals.Dispose();

    public sealed record SourceEvent(string Value) : IEvent;
    public sealed record TargetEvent(string Value) : IEvent;
}
