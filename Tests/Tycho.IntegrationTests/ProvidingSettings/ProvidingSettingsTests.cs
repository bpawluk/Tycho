using Tycho.IntegrationTests.ProvidingSettings.SUT;
using Tycho.IntegrationTests.ProvidingSettings.SUT.Modules;
using Tycho.IntegrationTests.ProvidingSettings.SUT.Settings;

namespace Tycho.IntegrationTests.ProvidingSettings;

public class ProvidingSettingsTests : IAsyncLifetime
{
    private ITestApp _sut = null!;

    public async Task InitializeAsync()
    {
        _sut = await new TestApp().RunAsync();
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ProvidingSettings()
    {
        // Arrange
        // - no arrangement required

        // Act
        var alphaValue = await _sut.ExecuteAsync(new GetAlphaValueRequest());
        var betaValue = await _sut.ExecuteAsync(new GetBetaValueRequest());
        var gammaValue = await _sut.ExecuteAsync(new GetGammaValueRequest());

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