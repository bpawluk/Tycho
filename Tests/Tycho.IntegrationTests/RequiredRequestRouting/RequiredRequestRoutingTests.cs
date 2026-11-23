using Tycho.IntegrationTests.RequiredRequestRouting.SUT;

namespace Tycho.IntegrationTests.RequiredRequestRouting;

public sealed class RequiredRequestRoutingTests : IAsyncLifetime
{
    private readonly TestResult _result = new();
    private ITestApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = new TestApp(_result).CreateAppBuilder().Build();
        await _sut.StartAsync(TestContext.Current.CancellationToken);
    }

    [Fact(Timeout = 5000)]
    public async Task ForwardsRequiredRequest_ToSibling_InvokesDestinationOnce()
    {
        // Arrange
        var request = new PlainCommand("input");

        // Act
        await _sut.ExecuteAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(new Invocation("destination", "input"), Assert.Single(_result.Invocations));
    }

    [Fact(Timeout = 5000)]
    public async Task ForwardsRequiredRequestWithResponse_ToSibling_ReturnsResponse()
    {
        // Arrange
        var request = new PlainQuery("input");

        // Act
        string response = await _sut.ExecuteAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("handled:input", response);
        Assert.Equal(new Invocation("destination", "input"), Assert.Single(_result.Invocations));
    }

    [Fact(Timeout = 5000)]
    public async Task ForwardsMappedRequiredRequest_ToSibling_TransformsPayload()
    {
        // Arrange
        var request = new MappedCommand("input");

        // Act
        await _sut.ExecuteAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(new Invocation("destination", "mapped:input"), Assert.Single(_result.Invocations));
    }

    [Fact(Timeout = 5000)]
    public async Task ForwardsMappedRequiredRequestWithResponse_ToSibling_TransformsResponse()
    {
        // Arrange
        var request = new MappedQuery("input");

        // Act
        string response = await _sut.ExecuteAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("mapped-response:42", response);
        Assert.Equal(new Invocation("destination", "mapped:input"), Assert.Single(_result.Invocations));
    }

    [Fact(Timeout = 5000)]
    public async Task IgnoresRequiredRequest_InModule_CompletesWithoutDelivery()
    {
        // Arrange
        var request = new IgnoredCommand("input");

        // Act
        await _sut.ExecuteAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Empty(_result.Invocations);
    }

    [Fact(Timeout = 5000)]
    public async Task IgnoresRequiredRequestWithResponse_InModule_ReturnsDefault()
    {
        // Arrange
        var request = new IgnoredQuery("input");

        // Act
        string response = await _sut.ExecuteAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(response);
        Assert.Empty(_result.Invocations);
    }

    [Fact(Timeout = 5000)]
    public async Task IgnoresRequiredRequest_InApp_CompletesWithoutDelivery()
    {
        // Arrange
        var request = new DirectIgnoredCommand("input");

        // Act
        await _sut.ExecuteAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Empty(_result.Invocations);
    }

    [Fact(Timeout = 5000)]
    public async Task IgnoresRequiredRequestWithResponse_InApp_ReturnsDefault()
    {
        // Arrange
        var request = new DirectIgnoredQuery("input");

        // Act
        string response = await _sut.ExecuteAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(response);
        Assert.Empty(_result.Invocations);
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
