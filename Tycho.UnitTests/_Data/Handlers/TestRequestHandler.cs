using Tycho.UnitTests._Data.Requests;
using TychoV2.Requests;

namespace Tycho.UnitTests._Data.Handlers;

internal class TestRequestHandler
    : IHandle<TestRequest>
    , IHandle<TestRequestWithResponse, string>
{
    public Task Handle(TestRequest requestData, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public Task<string> Handle(TestRequestWithResponse requestData, CancellationToken cancellationToken ) =>
        Task.FromResult(default(string)!);
}
