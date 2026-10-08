using Microsoft.Extensions.Hosting;
using Moq;
using Tycho.Identity.Structure;
using Tycho.Modules;
using Tycho.Modules.Setup;
using Tycho.Requests.Broker;
using Tycho.Structure;
using Tycho.UnitTests._Data.Requests;

namespace Tycho.UnitTests.Modules.Setup;

public sealed class ModuleContractTests : IDisposable
{
    private readonly Internals _internals = new ModuleInternals(
        Host.CreateEmptyApplicationBuilder(default),
        new ControlPlane(InstanceIdentity.Create(typeof(ModuleContractTests))),
        typeof(ModuleContractTests));
    private readonly Mock<IRequestBroker> _broker = new();
    private readonly ModuleContract _sut;

    public ModuleContractTests()
    {
        _sut = new ModuleContract(_internals);
    }

    [Fact]
    public void ContractFulfillingBroker_BeforeConfiguration_Throws()
    {
        // Act
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => _sut.ContractFulfillingBroker);

        // Assert
        Assert.Equal("Contract fulfilling broker has not been defined yet.", exception.Message);
    }

    [Fact]
    public void Requires_WithoutResponse_WhenParentCannotExecute_Throws()
    {
        // Arrange
        _sut.WithContractFulfillment(_broker.Object);

        // Act
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(_sut.Requires<TestRequest>);

        // Assert
        Assert.Equal("Parent module does not handle the required TestRequest request", exception.Message);
        _broker.Verify(broker => broker.CanExecute<TestRequest>(), Times.Once);
    }

    [Fact]
    public void Requires_WithResponse_WhenParentCannotExecute_Throws()
    {
        // Arrange
        _sut.WithContractFulfillment(_broker.Object);

        // Act
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            _sut.Requires<TestRequestWithResponse, string>);

        // Assert
        Assert.Equal("Parent module does not handle the required TestRequestWithResponse request", exception.Message);
        _broker.Verify(broker => broker.CanExecute<TestRequestWithResponse, string>(), Times.Once);
    }

    [Fact]
    public void Requires_WithoutResponse_WhenParentCanExecute_ReturnsContract()
    {
        // Arrange
        _broker.Setup(broker => broker.CanExecute<TestRequest>()).Returns(true);
        _sut.WithContractFulfillment(_broker.Object);

        // Act
        IModuleContract result = _sut.Requires<TestRequest>();

        // Assert
        Assert.Same(_sut, result);
    }

    [Fact]
    public void Requires_WithResponse_WhenParentCanExecute_ReturnsContract()
    {
        // Arrange
        _broker.Setup(broker => broker.CanExecute<TestRequestWithResponse, string>()).Returns(true);
        _sut.WithContractFulfillment(_broker.Object);

        // Act
        IModuleContract result = _sut.Requires<TestRequestWithResponse, string>();

        // Assert
        Assert.Same(_sut, result);
    }

    public void Dispose() => _internals.Dispose();
}
