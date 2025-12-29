using Tycho.Modules;

namespace Tycho.IntegrationTests.ProvidingSettings.SUT.Settings;

internal class ModuleSettings : IModuleSettings
{
    public string AlphaValue { get; set; } = string.Empty;

    public string BetaValue { get; set; } = string.Empty;
}