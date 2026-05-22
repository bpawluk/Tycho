using Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Handlers;

internal class TestRequestUsingHelperExtensionStructureModuleHandler(IHelperExtensionModule moduleFacade)
    : IRequestHandler<TestRequestUsingHelperExtensionStructureModule, string>
{
    private readonly IHelperExtensionModule _moduleFacade = moduleFacade;

    public Task<string> HandleAsync(TestRequestUsingHelperExtensionStructureModule requestData, CancellationToken cancellationToken)
        => Task.FromResult(_moduleFacade.GetType().Name);
}
