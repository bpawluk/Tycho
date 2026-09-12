using Tycho.Requests;
using Tycho.UnitTests._Data.Requests;

namespace Tycho.UnitTests._Data.Handlers;

internal class TestRequestHandler
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
