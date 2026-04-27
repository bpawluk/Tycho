using Microsoft.Extensions.Logging;
using Tycho.IntegrationTests.ConfiguringLogging.SUT;
using Tycho.IntegrationTests.ConfiguringLogging.SUT.Modules;

namespace Tycho.IntegrationTests.ConfiguringLogging;

public sealed class ConfiguringLoggingTests : IAsyncLifetime
{
    private ITestApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = await new TestApp().WithLogging(ConfigureLogging).RunAsync();
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ProvidingSettings()
    {
        // Arrange
        var logger = (TestLogger)new TestLoggerProvider().CreateLogger(string.Empty)!;

        // Act
        await _sut.ExecuteAsync(new LogAppRequest(), TestContext.Current.CancellationToken);
        await _sut.ExecuteAsync(new LogAlphaRequest(), TestContext.Current.CancellationToken);
        await _sut.ExecuteAsync(new LogBetaRequest(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(3, logger.Logs.Count);
        Assert.Contains("App", logger.Logs);
        Assert.Contains("Alpha", logger.Logs);
        Assert.Contains("Beta", logger.Logs);
    }

    private void ConfigureLogging(ILoggingBuilder builder)
    {
        builder.ClearProviders();
        builder.AddProvider(new TestLoggerProvider());
    }

    public async ValueTask DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}