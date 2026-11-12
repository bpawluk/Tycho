namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;

[TychoId("motion-detected")]
public record MotionDetected(string Zone) : IReading;
