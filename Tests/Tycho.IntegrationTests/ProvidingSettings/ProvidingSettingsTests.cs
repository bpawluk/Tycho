using Tycho.IntegrationTests.ProvidingSettings.SUT;
using Tycho.IntegrationTests.ProvidingSettings.SUT.Modules;
using Tycho.IntegrationTests.ProvidingSettings.SUT.Settings;

namespace Tycho.IntegrationTests.ProvidingSettings;

public sealed class ProvidingSettingsTests : IAsyncLifetime
{
    private ITestApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = new TestApp().CreateAppBuilder().Build();
        await _sut.StartAsync(TestContext.Current.CancellationToken);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ProvidingSettings()
    {
        // Arrange
        // - no arrangement required

        // Act
        string alphaValue = await _sut.ExecuteAsync(new GetAlphaValueRequest(), TestContext.Current.CancellationToken);
        string betaValue = await _sut.ExecuteAsync(new GetBetaValueRequest(), TestContext.Current.CancellationToken);
        string gammaValue = await _sut.ExecuteAsync(new GetGammaValueRequest(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("Alpha", alphaValue);
        Assert.Equal("Beta", betaValue);
        Assert.Equal(new OtherSettings().Value, gammaValue);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _sut.StopAsync();
        }
        finally
        {
            _sut.Dispose();
        }
    }
}
