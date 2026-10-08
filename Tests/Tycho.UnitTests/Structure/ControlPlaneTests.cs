using Moq;
using Tycho.Events.Delivery;
using Tycho.Identity.Structure;
using Tycho.Structure;

namespace Tycho.UnitTests.Structure;

public sealed class ControlPlaneTests
{
    private readonly ControlPlane _sut = new(InstanceIdentity.Create(typeof(ControlPlaneTests)));

    [Fact]
    public void Constructor_WithMissingApplicationId_RejectsIt()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new ControlPlane(null!));
        Assert.Equal("applicationId", exception.ParamName);
    }

    [Fact]
    public void GetModule_WithRegisteredModule_ReturnsTheSameReference()
    {
        InstanceIdentity id = InstanceIdentity.Parse("registered-module");
        var module = new ModuleReference(new Mock<IDeliveryEndpoint>(MockBehavior.Strict).Object);
        _sut.RegisterModule(id, module);
        _sut.CompleteRegistration();

        Assert.Same(module, _sut.GetModule(id));
    }

    [Fact]
    public void GetModule_WithMissingModule_ThrowsInvalidOperationException()
    {
        _sut.CompleteRegistration();

        Assert.Throws<InvalidOperationException>(() => _sut.GetModule(InstanceIdentity.Parse("missing-module")));
    }

    [Fact]
    public void GetModule_WithMultipleRegisteredModules_ReturnsEachReference()
    {
        var modules = Enumerable.Range(0, 3).ToDictionary(
            index => InstanceIdentity.Parse($"module-{index}"),
            _ => new ModuleReference(new Mock<IDeliveryEndpoint>(MockBehavior.Strict).Object));
        foreach (var (id, module) in modules)
        {
            _sut.RegisterModule(id, module);
        }
        _sut.CompleteRegistration();

        foreach (var (id, module) in modules)
        {
            Assert.Same(module, _sut.GetModule(id));
        }
    }

    [Fact]
    public void RegisterModule_WithDuplicateDestination_RejectsAmbiguousRegistration()
    {
        InstanceIdentity id = InstanceIdentity.Parse("duplicate-module");
        var original = new ModuleReference(new Mock<IDeliveryEndpoint>(MockBehavior.Strict).Object);
        var duplicate = new ModuleReference(new Mock<IDeliveryEndpoint>(MockBehavior.Strict).Object);
        _sut.RegisterModule(id, original);

        Assert.Throws<InvalidOperationException>(() => _sut.RegisterModule(InstanceIdentity.Parse(id.Value), duplicate));

        _sut.CompleteRegistration();
        Assert.Same(original, _sut.GetModule(id));
    }
}
