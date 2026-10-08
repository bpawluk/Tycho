namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;

[TychoId("door-opened")]
public record DoorOpened(string Door) : IReading;
