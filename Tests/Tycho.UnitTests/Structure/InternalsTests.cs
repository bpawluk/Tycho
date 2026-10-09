using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Tycho.Identity.Structure;
using Tycho.Structure;
using Tycho.UnitTests._Utils;

namespace Tycho.UnitTests.Structure;

public class InternalsTests : IDisposable
{
    private readonly Internals _sut = new TestInternals();

    [Fact]
    public async Task StartAndStopAsync_LogCompletedLifecycleWithOwnerIdentity()
    {
        // Arrange
        var logger = new Mock<ILogger<Internals>>();
        logger.Setup(item => item.IsEnabled(LogLevel.Information)).Returns(true);

        _sut.GetHostBuilder().Services.AddSingleton(logger.Object);
        _sut.Build();

        // Act
        await _sut.StartAsync(TestContext.Current.CancellationToken);
        await _sut.StopAsync(TestContext.Current.CancellationToken);

        // Assert
        LogAssert.Logged(logger, LogLevel.Information, 1001, "TychoHostStarted", null, ("OwnerId", _sut.OwnerId.Value));
        LogAssert.Logged(logger, LogLevel.Information, 1002, "TychoHostStopped", null, ("OwnerId", _sut.OwnerId.Value));
    }

    [Fact]
    public async Task StartAsync_BeforeBuild_ThrowsInvalidOperationException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.StartAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task StopAsync_BeforeBuild_ThrowsInvalidOperationException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.StopAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public void GetService_BeforeBuild_ThrowsInvalidOperationException()
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _sut.GetService(typeof(object)));
    }

    [Fact]
    public void GetHostBuilder_AfterBuild_ThrowsInvalidOperationException()
    {
        // Arrange
        _sut.Build();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(_sut.GetHostBuilder);
    }

    [Fact]
    public void Dispose_CalledTwice_DisposesHostServicesOnce()
    {
        // Arrange
        _sut.GetHostBuilder().Services.AddSingleton<DisposableService>();
        _sut.Build();
        DisposableService disposable = _sut.GetRequiredService<DisposableService>();

        // Act
        _sut.Dispose();
        _sut.Dispose();

        // Assert
        Assert.Equal(1, disposable.DisposeCalls);
    }

    public void Dispose() => _sut.Dispose();

    private sealed class TestInternals() : Internals(
        Host.CreateEmptyApplicationBuilder(default),
        new ControlPlane(InstanceIdentity.Create(typeof(InternalsTests))),
        InstanceIdentity.Create(typeof(InternalsTests)))
    {
        protected override void PrepareForStart(CancellationToken cancellationToken)
        {
        }
    }

    private sealed class DisposableService : IDisposable
    {
        public int DisposeCalls { get; private set; }

        public void Dispose() => DisposeCalls++;
    }
}
