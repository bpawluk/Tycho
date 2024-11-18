using Tycho.IntegrationTests.RunningCleanupLogic.SUT;

namespace Tycho.IntegrationTests.RunningCleanupLogic;

public class RunningCleanupLogicTests
{
    [Fact(Timeout = 500)]
    public async Task TychoEnables_RunningCleanupLogic_InAppsAndModules()
    {
        // Arrange
        var sut = await new TestApp().Run();
        var testResult = TestResult.Instance;

        // Act
        sut.Dispose();

        // Assert
        Assert.True(testResult.AppCleanupPerformed);
        Assert.True(testResult.AlphaModuleCleanupPerformed);
        Assert.True(testResult.BetaModuleCleanupPerformed);
        Assert.True(testResult.GammaModuleCleanupPerformed);
    }
}