using Tycho.IntegrationTests.ProvidingSettings.SUT;
using Tycho.IntegrationTests.ProvidingSettings.SUT.Modules;
using Tycho.IntegrationTests.ProvidingSettings.SUT.Settings;
using Tycho.Structure;

namespace Tycho.IntegrationTests.ProvidingSettings;

public class ProvidingSettingsTests : IAsyncLifetime
{
    private IApp _sut = null!;

    public async Task InitializeAsync()
    {
        _sut = await new TestApp().Run();
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ProvidingSettings()
    {
        // Arrange
        // - no arrangement required

        // Act
        var alphaValue = await _sut.Execute<GetAlphaValueRequest, string>(new GetAlphaValueRequest());
        var betaValue = await _sut.Execute<GetBetaValueRequest, string>(new GetBetaValueRequest());
        var gammaValue = await _sut.Execute<GetGammaValueRequest, string>(new GetGammaValueRequest());

        // Assert
        Assert.Equal("Alpha", alphaValue);
        Assert.Equal("Beta", betaValue);
        Assert.Equal(new OtherSettings().Value, gammaValue);
    }

    public async Task DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}