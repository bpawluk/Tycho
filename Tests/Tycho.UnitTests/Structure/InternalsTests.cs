using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tycho.Structure;

namespace Tycho.UnitTests.Structure;

public class InternalsTests
{
    private readonly Internals _sut = new(typeof(InternalsTests), Host.CreateEmptyApplicationBuilder(default));

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

    private sealed class DisposableService : IDisposable
    {
        public int DisposeCalls { get; private set; }

        public void Dispose() => DisposeCalls++;
    }
}
