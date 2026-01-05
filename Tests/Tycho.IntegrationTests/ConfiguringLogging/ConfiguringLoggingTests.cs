using Microsoft.Extensions.Logging;
using Tycho.IntegrationTests.ConfiguringLogging.SUT;
using Tycho.IntegrationTests.ConfiguringLogging.SUT.Modules;
using Tycho.Structure;

namespace Tycho.IntegrationTests.ConfiguringLogging;

public class ConfiguringLoggingTests : IAsyncLifetime
{
    private IApp _sut = null!;

    public async Task InitializeAsync()
    {
        _sut = await new TestApp().WithLogging(ConfigureLogging).RunAsync();
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ProvidingSettings()
    {
        // Arrange
        var logger = (TestLogger)new TestLoggerProvider().CreateLogger(string.Empty)!;

        // Act
        await _sut.ExecuteAsync(new LogAppRequest());
        await _sut.ExecuteAsync(new LogAlphaRequest());
        await _sut.ExecuteAsync(new LogBetaRequest());

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

    public async Task DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}