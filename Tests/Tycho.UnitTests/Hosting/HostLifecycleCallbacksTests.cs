using Moq;
using Tycho.Hosting;

namespace Tycho.UnitTests.Hosting;

public sealed class HostLifecycleCallbacksTests
{
    [Fact]
    public async Task DefaultCallbacks_WithoutConfiguration_CompleteSuccessfully()
    {
        // Arrange
        var sut = new HostLifecycleCallbacks();
        IServiceProvider services = Mock.Of<IServiceProvider>();
        CancellationToken token = TestContext.Current.CancellationToken;

        // Act
        Task startup = sut.Startup(services, token);
        Task cleanup = sut.Cleanup(services, token);
        await startup;
        await cleanup;

        // Assert
        Assert.True(startup.IsCompletedSuccessfully);
        Assert.True(cleanup.IsCompletedSuccessfully);
    }
}
