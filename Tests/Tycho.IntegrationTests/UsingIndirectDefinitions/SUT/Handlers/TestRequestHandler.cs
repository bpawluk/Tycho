using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Handlers;

internal class TestRequestHandler
    : IRequestHandler<TestRequestFromLocalHelper, string>
    , IRequestHandler<TestRequestFromLocalStaticHelper, string>
    , IRequestHandler<TestRequestFromHelperClass, string>
    , IRequestHandler<TestRequestFromHelperStaticClass, string>
    , IRequestHandler<TestRequestFromHelperExtension, string>
{
    public Task<string> HandleAsync(TestRequestFromLocalHelper requestData, CancellationToken cancellationToken)
        => Task.FromResult(nameof(TestRequestFromLocalHelper));

    public Task<string> HandleAsync(TestRequestFromLocalStaticHelper requestData, CancellationToken cancellationToken)
        => Task.FromResult(nameof(TestRequestFromLocalStaticHelper));

    public Task<string> HandleAsync(TestRequestFromHelperClass requestData, CancellationToken cancellationToken)
        => Task.FromResult(nameof(TestRequestFromHelperClass));

    public Task<string> HandleAsync(TestRequestFromHelperStaticClass requestData, CancellationToken cancellationToken)
        => Task.FromResult(nameof(TestRequestFromHelperStaticClass));

    public Task<string> HandleAsync(TestRequestFromHelperExtension requestData, CancellationToken cancellationToken)
        => Task.FromResult(nameof(TestRequestFromHelperExtension));
}
