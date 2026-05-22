using Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Handlers;

internal class TestRequestUsingLocalStructureModuleHandler(ILocalHelperModule moduleFacade)
    : IRequestHandler<TestRequestUsingLocalStructureModule, string>
{
    private readonly ILocalHelperModule _moduleFacade = moduleFacade;

    public Task<string> HandleAsync(TestRequestUsingLocalStructureModule requestData, CancellationToken cancellationToken)
        => Task.FromResult(_moduleFacade.GetType().Name);
}
