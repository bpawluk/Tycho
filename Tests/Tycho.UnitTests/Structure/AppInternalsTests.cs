using Microsoft.Extensions.Hosting;
using Tycho.Identity.Structure;
using Tycho.Structure;

namespace Tycho.UnitTests.Structure;

public class AppInternalsTests
{
    [Fact]
    public void Constructor_WithOwnerDefinition_CreatesApplicationControlPlane()
    {
        // Arrange
        InstanceIdentity expectedId = InstanceIdentity.Create(typeof(AppInternalsTests));

        // Act
        using var sut = new AppInternals(Host.CreateEmptyApplicationBuilder(default), typeof(AppInternalsTests));

        // Assert
        Assert.Equal(expectedId, sut.OwnerId);
        Assert.Equal(expectedId, sut.ControlPlane.ApplicationId);
    }

    [Fact]
    public async Task StartAsync_AfterBuild_CompletesRegistration()
    {
        // Arrange
        using var sut = new AppInternals(Host.CreateEmptyApplicationBuilder(default), typeof(AppInternalsTests));
        sut.Build();

        // Act
        await sut.StartAsync(TestContext.Current.CancellationToken);

        // Assert
        sut.ControlPlane.EnsureRegistrationComplete();
        Assert.NotNull(sut.ControlPlane.GetModule(sut.OwnerId));
    }
}
