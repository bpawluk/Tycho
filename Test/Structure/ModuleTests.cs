using Tycho;
using Tycho.Structure.Modules;

namespace Test.Structure;

public class ModuleTests : BaseModuleTests
{
    protected override IModule CreateModuleUnderTest() => new Module();

    protected override void SetBroker() => (_module as Module)!.SetExternalBroker(_messageBrokerMock.Object);
}
