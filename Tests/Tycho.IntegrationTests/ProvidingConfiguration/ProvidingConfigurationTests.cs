using Microsoft.Extensions.Configuration;
using Tycho.IntegrationTests.ProvidingConfiguration.SUT;
using Tycho.IntegrationTests.ProvidingConfiguration.SUT.Modules;

namespace Tycho.IntegrationTests.ProvidingConfiguration;

public sealed class ProvidingConfigurationTests : IAsyncLifetime
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
        Assert.Equal(_appValue, appValue);
        Assert.Equal(_alphaValue, alphaValue);
        Assert.Equal(_betaValue, betaValue);
    }

    public async ValueTask DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}
