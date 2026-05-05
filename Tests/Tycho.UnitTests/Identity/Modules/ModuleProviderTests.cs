using Moq;
using Tycho.Identity.Modules;
using Tycho.Modules.Instance;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Identity.Modules;

public class ModuleProviderTests
{
    private readonly Dictionary<ModuleIdentity, IModule> _registeredModules = [];

    private readonly ModuleProvider _sut;

    public ModuleProviderTests()
    {
        var firstModuleMock = new Mock<IModule>();
        var firstModule = firstModuleMock.Object;
        var firstModuleId = ModuleIdentity.Create<OtherModule>();
        _registeredModules.Add(firstModuleId, firstModule);
        firstModuleMock.SetupGet(m => m.Identity).Returns(firstModuleId);

        var secondModuleMock = new Mock<IModule>();
        var secondModule = secondModuleMock.Object;
        var secondModuleId = ModuleIdentity.Create<TestModule>();
        _registeredModules.Add(secondModuleId, secondModule);
        secondModuleMock.SetupGet(m => m.Identity).Returns(secondModuleId);

        var thirdModuleMock = new Mock<IModule>();
        var thirdModule = thirdModuleMock.Object;
        var thirdModuleId = ModuleIdentity.Create<AnotherModule>();
        _registeredModules.Add(thirdModuleId, thirdModule);
        thirdModuleMock.SetupGet(m => m.Identity).Returns(thirdModuleId);

        _sut = new ModuleProvider(_registeredModules.Values);
    }

    [Fact]
    public void GetModule_WithRegisteredModule_ReturnsTheModule()
    {
        // Arrange
        var moduleId = _registeredModules.Keys.ElementAt(1);
        var module = _registeredModules[moduleId];

        // Act
        var result = _sut.GetModule(moduleId);

        // Assert
        Assert.Same(module, result);
    }

    [Fact]
    public void GetModule_WithMissingModule_ThrowsArgumentException()
    {
        // Arrange
        var missingId = ModuleIdentity.Create<YetAnotherModule>();

        // Act 
        void Act() => _sut.GetModule(missingId);

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void GetAllModules_ReturnsAllRegisteredModules()
    {
        // Arrange
        var allModules = _registeredModules.Values.ToList();

        // Act
        var modules = _sut.GetAllModules();

        // Assert
        Assert.Equal(allModules.Count, modules.Count);
        foreach (var module in allModules)
        {
            Assert.Contains(module, modules);
        }
    }
}
