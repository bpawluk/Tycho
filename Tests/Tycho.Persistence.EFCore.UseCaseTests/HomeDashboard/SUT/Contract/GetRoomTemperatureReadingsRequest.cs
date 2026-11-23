using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Contract;
using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract;

public record GetRoomTemperatureReadingsRequest(string Room) : IRequest<GetTemperatureReadingsRequest.Response>
{
}
