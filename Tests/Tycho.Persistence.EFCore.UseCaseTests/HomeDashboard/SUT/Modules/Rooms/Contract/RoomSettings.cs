using Tycho.Modules;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Rooms.Contract;

public sealed class RoomSettings : IModuleSettings
{
    public string Room { get; init; } = string.Empty;
}
