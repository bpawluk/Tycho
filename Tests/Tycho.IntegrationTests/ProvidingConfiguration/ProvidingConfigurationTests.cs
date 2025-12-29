using Microsoft.Extensions.Configuration;
using Tycho.IntegrationTests.ProvidingConfiguration.SUT;
using Tycho.IntegrationTests.ProvidingConfiguration.SUT.Modules;
using Tycho.Structure;

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

    private IApp _sut = null!;

    public async Task InitializeAsync()
    {
        var builtAppConfig = new ConfigurationBuilder().AddInMemoryCollection(_appConfig).Build();
        _sut = await new TestApp().WithConfiguration(builtAppConfig).Run();
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ProvidingConfiguration()
    {
        // Arrange
        // - no arrangement required

        // Act
        var appValue = await _sut.Execute<GetAppValueRequest, string>(new GetAppValueRequest());
        var alphaValue = await _sut.Execute<GetAlphaValueRequest, string>(new GetAlphaValueRequest());
        var betaValue = await _sut.Execute<GetBetaValueRequest, string>(new GetBetaValueRequest());

        // Assert
        Assert.Equal(_appValue, appValue);
        Assert.Equal(_alphaValue, alphaValue);
        Assert.Equal(_betaValue, betaValue);
    }

    public async Task DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}