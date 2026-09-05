using Microsoft.Extensions.Hosting;
using Tycho.Apps.Setup;
using Tycho.Structure;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Apps.Setup;

public class AppStructureTests
{
    private readonly Internals _internals;
    private readonly AppStructure _sut;

    public AppStructureTests()
    {
        _internals = new Internals(typeof(object), Host.CreateEmptyApplicationBuilder(default));
        _sut = new AppStructure(_internals);
    }

    [Fact]
    public void Uses_SameModuleTwice_ThrowsInvalidOperationException()
    {
        // Arrange
        _sut.Uses<TestModule>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _sut.Uses<TestModule>());
    }

    [Fact]
    public void Uses_WithContractFulfillmentAction_InvokesTheAction()
    {
        // Arrange
        bool actionInvoked = false;

        // Act
        _sut.Uses<TestModule>(_ => { actionInvoked = true; });

        // Assert
        Assert.True(actionInvoked);
    }
}
