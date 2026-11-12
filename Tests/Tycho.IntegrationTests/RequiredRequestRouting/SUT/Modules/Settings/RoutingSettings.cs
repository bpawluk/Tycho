using Tycho.Modules;

namespace Tycho.IntegrationTests.RequiredRequestRouting.SUT.Modules.Settings;

public sealed class RoutingSettings : IModuleSettings
{
    public TestResult Result { get; init; } = new();
}
