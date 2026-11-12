using Tycho.Requests;

namespace Tycho.IntegrationTests.RequiredRequestRouting.SUT.Modules.Handlers;

public sealed record DestinationName(string Value);

internal sealed class DestinationRequestHandler(TestResult result, DestinationName destination)
    : IRequestHandler<PlainCommand>, IRequestHandler<PlainQuery, string>,
      IRequestHandler<TargetCommand>, IRequestHandler<TargetQuery, int>,
      IRequestHandler<IgnoredCommand>, IRequestHandler<IgnoredQuery, string>
{
    public Task HandleAsync(PlainCommand request, CancellationToken token)
    {
        result.Invocations.Add(new(destination.Value, request.Value));
        return Task.CompletedTask;
    }

    public Task<string> HandleAsync(PlainQuery request, CancellationToken token)
    {
        result.Invocations.Add(new(destination.Value, request.Value));
        return Task.FromResult("handled:" + request.Value);
    }

    public Task HandleAsync(TargetCommand request, CancellationToken token)
    {
        result.Invocations.Add(new(destination.Value, request.Value));
        return Task.CompletedTask;
    }

    public Task<int> HandleAsync(TargetQuery request, CancellationToken token)
    {
        result.Invocations.Add(new(destination.Value, request.Value));
        return Task.FromResult(42);
    }

    public Task HandleAsync(IgnoredCommand request, CancellationToken token)
    {
        result.Invocations.Add(new(destination.Value, request.Value));
        return Task.CompletedTask;
    }

    public Task<string> HandleAsync(IgnoredQuery request, CancellationToken token)
    {
        result.Invocations.Add(new(destination.Value, request.Value));
        return Task.FromResult("unexpected response");
    }
}
