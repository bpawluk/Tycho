using Microsoft.Extensions.Configuration;
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

    public async ValueTask InitializeAsync()
    {
        IConfigurationRoot builtAppConfig = new ConfigurationBuilder().AddInMemoryCollection(_appConfig).Build();
        _sut = await new TestApp().WithConfiguration(builtAppConfig).RunAsync();
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
        await _sut!.DisposeAsync();
    }
}
