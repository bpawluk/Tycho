using Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Handlers;

internal class TestRequestUsingLocalStaticStructureModuleHandler(ILocalStaticHelperModule moduleFacade)
    : IRequestHandler<TestRequestUsingLocalStaticStructureModule, string>
{
    private readonly ILocalStaticHelperModule _moduleFacade = moduleFacade;

    public Task<string> HandleAsync(TestRequestUsingLocalStaticStructureModule requestData, CancellationToken cancellationToken)
        => Task.FromResult(_moduleFacade.GetType().Name);
}
