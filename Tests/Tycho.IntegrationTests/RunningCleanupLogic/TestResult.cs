namespace Tycho.IntegrationTests.RunningCleanupLogic;

public record TestResult
{
    public static TestResult Instance { get; } = new TestResult();

    public bool AppCleanupPerformed { get; set; } = false;

    public bool AlphaModuleCleanupPerformed { get; set; } = false;

    public bool BetaModuleCleanupPerformed { get; set; } = false;

    public bool GammaModuleCleanupPerformed { get; set; } = false;
}