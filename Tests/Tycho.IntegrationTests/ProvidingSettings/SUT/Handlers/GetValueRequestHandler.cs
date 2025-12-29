using Tycho.IntegrationTests.ProvidingSettings.SUT.Modules;
using Tycho.IntegrationTests.ProvidingSettings.SUT.Settings;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ProvidingSettings.SUT.Handlers;

internal class GetValueRequestHandler(ModuleSettings moduleSettings, OtherSettings otherSettings)
    : IRequestHandler<GetAlphaValueRequest, string>
    , IRequestHandler<GetBetaValueRequest, string>
    , IRequestHandler<GetGammaValueRequest, string>
{
    private readonly ModuleSettings _moduleSettings = moduleSettings;
    private readonly OtherSettings _otherSettings = otherSettings;

    public Task<string> Handle(GetAlphaValueRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_moduleSettings.AlphaValue);
    }

    public Task<string> Handle(GetBetaValueRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_moduleSettings.BetaValue);
    }

    public Task<string> Handle(GetGammaValueRequest requestData, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_otherSettings.Value);
    }
}