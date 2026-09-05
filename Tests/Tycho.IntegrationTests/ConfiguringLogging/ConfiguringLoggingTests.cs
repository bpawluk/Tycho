using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tycho.IntegrationTests.ConfiguringLogging.SUT;
using Tycho.IntegrationTests.ConfiguringLogging.SUT.Modules;

namespace Tycho.IntegrationTests.ConfiguringLogging;

public sealed class ConfiguringLoggingTests : IAsyncLifetime
{
    private ITestApp _sut = null!;
    private IHost _host = null!;

    public async ValueTask InitializeAsync()
    {
        var builder = new HostApplicationBuilder();
        ConfigureLogging(builder.Logging);
        builder.AddTestApp(new());

        _host = builder.Build();
        await _host.StartAsync(TestContext.Current.CancellationToken);
        _sut = _host.Services.GetRequiredService<ITestApp>();
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

    private static void ConfigureLogging(ILoggingBuilder builder)
    {
        builder.ClearProviders();
        builder.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.None);
        builder.AddProvider(new TestLoggerProvider());
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _host.StopAsync();
        }
        finally
        {
            _host.Dispose();
        }
    }
}
