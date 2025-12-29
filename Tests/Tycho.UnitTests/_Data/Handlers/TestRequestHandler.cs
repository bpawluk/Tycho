using Tycho.Requests;
using Tycho.UnitTests._Data.Requests;

namespace Tycho.UnitTests._Data.Handlers;

internal class TestRequestHandler
    : IRequestHandler<TestRequest>
        , IRequestHandler<TestRequestWithResponse, string>
{
    public Task Handle(TestRequest requestData, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task<string> Handle(TestRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(default(string)!);
    }
}