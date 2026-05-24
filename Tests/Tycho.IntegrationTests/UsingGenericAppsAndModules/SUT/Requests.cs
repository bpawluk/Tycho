using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT;

public sealed record AppWorkflowRequest(TestResult Result) : IRequest<string>;

public sealed record ModuleWorkflowRequest(TestResult Result) : IRequest<string>;

public sealed record CompleteWorkflowRequest(TestResult Result) : IRequest<string>;
