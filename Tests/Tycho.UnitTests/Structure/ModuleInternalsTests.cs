using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tycho.Identity.Structure;
using Tycho.Structure;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Structure;

public class ModuleInternalsTests
{
    [Fact]
    public void Constructor_WithSameDefinitionAndSuffixInDifferentApplications_CreatesSameOwnerId()
    {
        // Arrange
        var firstControlPlane = new ControlPlane(InstanceIdentity.Create(typeof(TestModule)));
        var secondControlPlane = new ControlPlane(InstanceIdentity.Create(typeof(OtherModule)));

        // Act
        using var firstModule = new ModuleInternals(
            Host.CreateEmptyApplicationBuilder(null),
            firstControlPlane, typeof(TestModule), "shared");
        using var secondModule = new ModuleInternals(
            Host.CreateEmptyApplicationBuilder(null),
            secondControlPlane, typeof(TestModule), "shared");

        // Assert
        Assert.NotEqual(firstControlPlane.ApplicationId, secondControlPlane.ApplicationId);
        Assert.Equal(firstModule.OwnerId, secondModule.OwnerId);
    }

    [Fact]
    public async Task StartAsync_WithIncompleteRegistration_ThrowsInvalidOperationException()
    {
        // Arrange
        var controlPlane = new ControlPlane(InstanceIdentity.Create(typeof(ModuleInternalsTests)));
        using var sut = new ModuleInternals(Host.CreateEmptyApplicationBuilder(default), controlPlane, typeof(TestModule));
        sut.Build();

        // Act
        Task Act() => sut.StartAsync(TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
    }

    [Fact]
    public async Task StartAsync_WithCompleteRegistration_StartsHost()
    {
        // Arrange
        var controlPlane = new ControlPlane(InstanceIdentity.Create(typeof(ModuleInternalsTests)));
        using var sut = new ModuleInternals(Host.CreateEmptyApplicationBuilder(default), controlPlane, typeof(TestModule));
        sut.Build();
        controlPlane.CompleteRegistration();
        IHostApplicationLifetime lifetime = sut.GetRequiredService<IHostApplicationLifetime>();

        // Act
        await sut.StartAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.True(lifetime.ApplicationStarted.IsCancellationRequested);
    }
}
