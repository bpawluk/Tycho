using Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Handlers;

internal class TestRequestUsingHelperClassStructureModuleHandler(IHelperClassModule moduleFacade)
    : IRequestHandler<TestRequestUsingHelperClassStructureModule, string>
{
    private readonly IHelperClassModule _moduleFacade = moduleFacade;

    public Task<string> HandleAsync(TestRequestUsingHelperClassStructureModule requestData, CancellationToken cancellationToken)
        => Task.FromResult(_moduleFacade.GetType().Name);
}
