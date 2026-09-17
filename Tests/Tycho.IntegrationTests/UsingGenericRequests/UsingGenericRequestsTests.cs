using Tycho.IntegrationTests.UsingGenericRequests.SUT;

namespace Tycho.IntegrationTests.UsingGenericRequests;

public sealed class UsingGenericRequestsTests : IAsyncLifetime
{
    private ITestApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = new TestApp().CreateAppBuilder().Build();
        await _sut.StartAsync(TestContext.Current.CancellationToken);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_SendingClosedGenericRequests()
    {
        // Act
        GenericAppRequest<int>.Response<int> intResponse = await _sut.ExecuteAsync(
            new GenericAppRequest<int>(123),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(123, intResponse.Data);

        // Act
        GenericAppRequest<string>.Response<string> stringResponse = await _sut.ExecuteAsync(
            new GenericAppRequest<string>("generic-app-request"),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("generic-app-request", stringResponse.Data);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ForwardingClosedGenericRequests()
    {
        // Act
        GenericAppRequestToForward<int>.Response<int> intResponse = await _sut.ExecuteAsync(
            new GenericAppRequestToForward<int>(456),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(456, intResponse.Data);

        // Act
        GenericAppRequestToForward<string>.Response<string> stringResponse = await _sut.ExecuteAsync(
            new GenericAppRequestToForward<string>("generic-forwarded-request"),
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("generic-forwarded-request", stringResponse.Data);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _sut.StopAsync();
        }
        finally
        {
            _sut.Dispose();
        }
    }
}
