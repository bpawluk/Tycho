using Tycho.Modules;

namespace Tycho.IntegrationTests.ProvidingSettings.SUT.Settings;

internal class OtherSettings : IModuleSettings
{
    public string Value { get; set; } = "Default";
}