using Tycho.Apps.Setup;
using Tycho.Structure;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Apps.Setup;

public class AppStructureTests
{
    private readonly Internals _internals;
    private readonly Globals _globals;
    private readonly AppStructure _sut;

    public AppStructureTests()
    {
        _internals = new Internals(typeof(object));
        _globals = new Globals();
        _sut = new AppStructure(_internals, _globals);
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
