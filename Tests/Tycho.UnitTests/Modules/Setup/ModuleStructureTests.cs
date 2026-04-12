using Tycho.Modules.Setup;
using Tycho.Structure;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Modules.Setup;

public class ModuleStructureTests
{
    private readonly Internals _internals;
    private readonly Globals _globals;
    private readonly ModuleStructure _sut;

    public ModuleStructureTests()
    {
        _internals = new Internals(typeof(object));
        _globals = new Globals();
        _sut = new ModuleStructure(_internals, _globals);
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
        var actionInvoked = false;

        // Act
        _sut.Uses<TestModule>(_ => { actionInvoked = true; });

        // Assert
        Assert.True(actionInvoked);
    }
}
