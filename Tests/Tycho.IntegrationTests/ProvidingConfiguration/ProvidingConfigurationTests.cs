using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tycho.IntegrationTests.ProvidingConfiguration.SUT;
using Tycho.IntegrationTests.ProvidingConfiguration.SUT.Modules;

namespace Tycho.IntegrationTests.ProvidingConfiguration;

public sealed class ProvidingConfigurationTests : IAsyncLifetime
{
    private const string AppValue = "App";
    private const string AlphaValue = "Alpha";
    private const string BetaValue = "Beta";

    private readonly Dictionary<string, string?> _appConfig = new()
    {
        ["App:Value"] = AppValue,
        ["Alpha:Value"] = AlphaValue,
        ["Beta:Value"] = BetaValue
    };

    private ITestApp _sut = null!;
    private IHost _host = null!;

    public async ValueTask InitializeAsync()
    {
        IConfigurationRoot builtAppConfig = new ConfigurationBuilder().AddInMemoryCollection(_appConfig).Build();
        var builder = new HostApplicationBuilder();
        builder.Configuration.AddConfiguration(builtAppConfig);
        builder.AddTestApp(new());

        _host = builder.Build();
        await _host.StartAsync(TestContext.Current.CancellationToken);
        _sut = _host.Services.GetRequiredService<ITestApp>();
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ProvidingConfiguration()
    {
        // Arrange
        // - no arrangement required

        // Act
        string appValue = await _sut.ExecuteAsync(new GetAppValueRequest(), TestContext.Current.CancellationToken);
        string alphaValue = await _sut.ExecuteAsync(new GetAlphaValueRequest(), TestContext.Current.CancellationToken);
        string betaValue = await _sut.ExecuteAsync(new GetBetaValueRequest(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(AppValue, appValue);
        Assert.Equal(AlphaValue, alphaValue);
        Assert.Equal(BetaValue, betaValue);
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
