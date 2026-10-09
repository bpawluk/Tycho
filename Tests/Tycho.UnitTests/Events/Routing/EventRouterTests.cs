using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Tycho.Events.Model;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.Identity.Structure;
using Tycho.Structure;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Routing;

public sealed class EventRouterTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RouteAsync_UsesFreshScopeAndAwaitsDisposal(bool fails)
    {
        using var internals = new AppInternals(Host.CreateEmptyApplicationBuilder(default), typeof(EventRouterTests));
        var publishId = Guid.NewGuid();
        var payload = new TestEvent();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        IReadOnlyCollection<RoutedEvent> expected = [new RoutedEvent<TestEvent>(
            Guid.NewGuid(), publishId, EventIdentity.Create<TestEvent>(),
            EventHandlerIdentity.Create<TestEventHandler>(), InstanceIdentity.Parse("destination"), payload)];
        var failure = new InvalidOperationException("Routing failed.");
        var scopes = new List<AsyncScopeProbe>();
        var router = new Mock<IEventRouter>(MockBehavior.Strict);
        router.Setup(item => item.RouteAsync(publishId, payload, cancellationToken))
            .Returns(() =>
            {
                Assert.False(scopes[^1].Disposed);
                return fails ? Task.FromException<IReadOnlyCollection<RoutedEvent>>(failure) : Task.FromResult(expected);
            });
        // Resolve the probe as a scoped dependency so DI owns its async disposal.
        internals.GetHostBuilder().Services.AddScoped<AsyncScopeProbe>(_ =>
        {
            var probe = new AsyncScopeProbe();
            scopes.Add(probe);
            return probe;
        });
        internals.GetHostBuilder().Services.AddScoped<IEventRouter>(provider =>
        {
            provider.GetRequiredService<AsyncScopeProbe>();
            return router.Object;
        });
        internals.Build();
        var sut = new EventRouter(internals);

        for (int call = 0; call < 2; call++)
        {
            if (fails)
            {
                InvalidOperationException actual = await Assert.ThrowsAsync<InvalidOperationException>(
                    () => sut.RouteAsync(publishId, payload, cancellationToken));
                Assert.Same(failure, actual);
            }
            else
            {
                Assert.Same(expected, await sut.RouteAsync(publishId, payload, cancellationToken));
            }

            Assert.Equal(call + 1, scopes.Count);
            Assert.All(scopes, scope => Assert.True(scope.Disposed));
        }

        Assert.NotSame(scopes[0], scopes[1]);
        router.Verify(item => item.RouteAsync(publishId, payload, cancellationToken), Times.Exactly(2));
        router.VerifyNoOtherCalls();
    }

    private sealed class AsyncScopeProbe : IAsyncDisposable
    {
        public bool Disposed { get; private set; }

        public async ValueTask DisposeAsync()
        {
            await Task.Yield();
            Disposed = true;
        }
    }
}
