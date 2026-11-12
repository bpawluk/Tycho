using Tycho.IntegrationTests.InterceptingRequests.SUT;

namespace Tycho.IntegrationTests.InterceptingRequests;

public sealed class InterceptingRequestsTests : IAsyncLifetime
{
    private ITestApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = new TestApp().CreateAppBuilder().Build();
        await _sut.StartAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task TychoEnables_InterceptingRequests()
    {
        // Arrange
        var trace = new List<string>();

        // Act
        await _sut.ExecuteAsync(new RequestToIntercept(trace), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(["app-before", "module-before", "module-handler", "app-before", "app-handler", "app-after", "module-after", "app-after"], trace);
    }

    [Fact]
    public async Task TychoEnables_InterceptingRequestsWithResponses()
    {
        // Arrange
        var trace = new List<string>();

        // Act
        string response = await _sut.ExecuteAsync(new RequestWithResponseToIntercept(trace), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("response", response);
        Assert.Equal(["app-before", "module-before", "module-handler", "app-before", "app-handler", "app-after", "module-after", "app-after"], trace);
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
