using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.UsingGenericEvents.SUT;

namespace Tycho.IntegrationTests.UsingGenericEvents;

public sealed class UsingGenericEventsTests : IAsyncLifetime
{
    private readonly TestWorkflow<GenericEventResult<int>> _intWorkflow = new();
    private readonly TestWorkflow<GenericEventResult<string>> _stringWorkflow = new();
    private ITestApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = await new TestApp(_intWorkflow, _stringWorkflow).RunAsync();
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_SendingClosedGenericEvents()
    {
        // Act
        await _sut.ExecuteAsync(new PublishGenericAppIntEventRequest(123), TestContext.Current.CancellationToken);
        GenericEventResult<int> intResult = await _intWorkflow.GetResult();

        // Assert
        Assert.Equal("app", intResult.Path);
        Assert.Equal(123, intResult.Data);

        // Act
        await _sut.ExecuteAsync(new PublishGenericAppStringEventRequest("generic-app-event"), TestContext.Current.CancellationToken);
        GenericEventResult<string> stringResult = await _stringWorkflow.GetResult();

        // Assert
        Assert.Equal("app", stringResult.Path);
        Assert.Equal("generic-app-event", stringResult.Data);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ForwardingClosedGenericEvents()
    {
        // Act
        await _sut.ExecuteAsync(new PublishGenericForwardedIntEventRequest(456), TestContext.Current.CancellationToken);
        GenericEventResult<int> intResult = await _intWorkflow.GetResult();

        // Assert
        Assert.Equal("forwarded", intResult.Path);
        Assert.Equal(456, intResult.Data);

        // Act
        await _sut.ExecuteAsync(new PublishGenericForwardedStringEventRequest("generic-forwarded-event"), TestContext.Current.CancellationToken);
        GenericEventResult<string> stringResult = await _stringWorkflow.GetResult();

        // Assert
        Assert.Equal("forwarded", stringResult.Path);
        Assert.Equal("generic-forwarded-event", stringResult.Data);
    }

    public async ValueTask DisposeAsync()
    {
        await _sut.DisposeAsync();
    }
}
