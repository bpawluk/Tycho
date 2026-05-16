using Tycho.IntegrationTests.RunningCleanupLogic.SUT;

namespace Tycho.IntegrationTests.RunningCleanupLogic;

public class RunningCleanupLogicTests
{
    [Fact(Timeout = 5000)]
    public async Task TychoEnables_RunningCleanupLogic_InAppsAndModules()
    {
        // Arrange
        ITestApp sut = await new TestApp().RunAsync();
        TestResult testResult = TestResult.Instance;

        // Act
        await sut.DisposeAsync();

        // Assert
        Assert.True(testResult.AppCleanupPerformed);
        Assert.True(testResult.AlphaModuleCleanupPerformed);
        Assert.True(testResult.BetaModuleCleanupPerformed);
        Assert.True(testResult.GammaModuleCleanupPerformed);
    }
}
