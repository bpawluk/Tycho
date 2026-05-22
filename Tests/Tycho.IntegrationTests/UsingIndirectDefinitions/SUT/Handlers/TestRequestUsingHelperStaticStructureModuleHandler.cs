using Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Handlers;

internal class TestRequestUsingHelperStaticStructureModuleHandler(IHelperStaticClassModule moduleFacade)
    : IRequestHandler<TestRequestUsingHelperStaticStructureModule, string>
{
    private readonly IHelperStaticClassModule _moduleFacade = moduleFacade;

    public Task<string> HandleAsync(TestRequestUsingHelperStaticStructureModule requestData, CancellationToken cancellationToken)
        => Task.FromResult(_moduleFacade.GetType().Name);
}
