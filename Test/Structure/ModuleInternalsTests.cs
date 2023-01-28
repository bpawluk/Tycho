using System;
using Test.Utils;
using Tycho;
using Tycho.Structure.Modules;

namespace Test.Structure;

public class ModuleInternalsTests : BaseModuleTests
{
    private Module? _actualModule;
    private ModuleInternals ModuleInternals => (_module as ModuleInternals)!;

    protected override IModule CreateModuleUnderTest()
    {
        _actualModule = new Module();
        return _actualModule.Internals;
    }

    protected override void SetBroker() => _actualModule!.SetInternalBroker(_messageBrokerMock.Object);

    [Fact]
    public void GetSubmodule_NoSubmodulesRegistered_ThrowsInvalidOperationException()
    {
        // Arrange
        // - no arrangement required

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => ModuleInternals.GetSubmodule<TestModule>());
    }

    [Fact]
    public void GetSubmodule_WhichWasNotRegistered_ThrowsInvalidOperationException()
    {
        // Arrange
        var submodules = new IModule[]
        {
            new Module<OtherModule>(),
            new Module<YetAnotherModule>()
        };
        _actualModule!.SetSubmodules(submodules);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => ModuleInternals.GetSubmodule<TestModule>());
    }

    [Fact]
    public void SetSubmodules_ContainsDuplicates_ThrowsInvalidOperationException()
    {
        // Arrange
        var submodules = new IModule[]
        {
            new Module<TestModule>(),
            new Module<TestModule>(),
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _actualModule!.SetSubmodules(submodules));
    }

    [Fact]
    public void SetSubmodules_SubmodulesAlreadyDefined_ThrowsInvalidOperationException()
    {
        // Arrange
        var submodules = new IModule[]
        {
            new Module<TestModule>(),
            new Module<OtherModule>(),
            new Module<YetAnotherModule>()
        };
        _actualModule!.SetSubmodules(submodules);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _actualModule!.SetSubmodules(submodules));
    }

    [Fact]
    public void SetSubmodules_NoSubmodulesPreviouslyDefined_DefinesAllSubmodules()
    {
        // Arrange
        var submodules = new IModule[]
        {
            new Module<TestModule>(),
            new Module<OtherModule>(),
            new Module<YetAnotherModule>()
        };

        // Act
        _actualModule!.SetSubmodules(submodules);

        // Assert
        Assert.Equal(submodules[0], ModuleInternals.GetSubmodule<TestModule>());
        Assert.Equal(submodules[1], ModuleInternals.GetSubmodule<OtherModule>());
        Assert.Equal(submodules[2], ModuleInternals.GetSubmodule<YetAnotherModule>());
    }
}
