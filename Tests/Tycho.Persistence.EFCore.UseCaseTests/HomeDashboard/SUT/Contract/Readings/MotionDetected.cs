namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Contract.Readings;

public record MotionDetected(string Zone) : IReading;
