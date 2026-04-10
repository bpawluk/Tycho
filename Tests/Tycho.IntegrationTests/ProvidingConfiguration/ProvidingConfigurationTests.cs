using Microsoft.Extensions.Configuration;
using Tycho.IntegrationTests.ProvidingConfiguration.SUT;
using Tycho.IntegrationTests.ProvidingConfiguration.SUT.Modules;

namespace Tycho.IntegrationTests.ProvidingConfiguration;

public class ProvidingConfigurationTests : IAsyncLifetime
{
    private const string _appValue = "App";
    private const string _alphaValue = "Alpha";
    private const string _betaValue = "Beta";

    private readonly Dictionary<string, string?> _appConfig = new()
    {
        ["App:Value"] = _appValue,
        ["Alpha:Value"] = _alphaValue,
        ["Beta:Value"] = _betaValue
    };

    private ITestApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        var builtAppConfig = new ConfigurationBuilder().AddInMemoryCollection(_appConfig).Build();
        _sut = await new TestApp().WithConfiguration(builtAppConfig).RunAsync();
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ProvidingConfiguration()
    {
        // Arrange
        // - no arrangement required

        // Act
        var appValue = await _sut.ExecuteAsync(new GetAppValueRequest());
        var alphaValue = await _sut.ExecuteAsync(new GetAlphaValueRequest());
        var betaValue = await _sut.ExecuteAsync(new GetBetaValueRequest());

        // Assert
        Assert.Equal(_appValue, appValue);
        Assert.Equal(_alphaValue, alphaValue);
        Assert.Equal(_betaValue, betaValue);
    }

    public async ValueTask DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}