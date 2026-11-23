using Tycho.Requests;
using Tycho.TestUtils.Requests;

namespace Tycho.TestUtils.Handlers;

public class TestRequestHandler
    : IRequestHandler<TestRequest>
    , IRequestHandler<TestRequestWithResponse, string>
{
    public Task HandleAsync(TestRequest requestData, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task<string> HandleAsync(TestRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(default(string)!);
    }
}
