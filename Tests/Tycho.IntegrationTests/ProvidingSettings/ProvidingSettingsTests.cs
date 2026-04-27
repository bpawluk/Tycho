using Tycho.IntegrationTests.ProvidingSettings.SUT;
using Tycho.IntegrationTests.ProvidingSettings.SUT.Modules;
using Tycho.IntegrationTests.ProvidingSettings.SUT.Settings;

namespace Tycho.IntegrationTests.ProvidingSettings;

public sealed class ProvidingSettingsTests : IAsyncLifetime
{
    private ITestApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = await new TestApp().RunAsync();
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ProvidingSettings()
    {
        // Arrange
        // - no arrangement required

        // Act
        var alphaValue = await _sut.ExecuteAsync(new GetAlphaValueRequest(), TestContext.Current.CancellationToken);
        var betaValue = await _sut.ExecuteAsync(new GetBetaValueRequest(), TestContext.Current.CancellationToken);
        var gammaValue = await _sut.ExecuteAsync(new GetGammaValueRequest(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("Alpha", alphaValue);
        Assert.Equal("Beta", betaValue);
        Assert.Equal(new OtherSettings().Value, gammaValue);
    }

    public async ValueTask DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}