using Tycho.IntegrationTests.RunningCleanupLogic.SUT;

namespace Tycho.IntegrationTests.RunningCleanupLogic;

public class RunningCleanupLogicTests
{
    [Fact(Timeout = 5000)]
    public async Task TychoEnables_RunningCleanupLogic_InAppsAndModules()
    {
        // Arrange
        using ITestApp sut = new TestApp().CreateAppBuilder().Build();
        await sut.StartAsync(TestContext.Current.CancellationToken);
        TestResult testResult = TestResult.Instance;

        // Act
        await sut.StopAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.True(testResult.AppCleanupPerformed);
        Assert.True(testResult.AlphaModuleCleanupPerformed);
        Assert.True(testResult.BetaModuleCleanupPerformed);
        Assert.True(testResult.GammaModuleCleanupPerformed);
    }
}
