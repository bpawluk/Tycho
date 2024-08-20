using System.Threading.Tasks;
using System.Threading;
using Tycho.Messaging.Handlers;

namespace UnitTests.Utils;

public record TestMessageHandler()
    : IEventHandler<TestEvent>
    , IRequestHandler<TestRequest>
    , IRequestHandler<TestRequestWithResponse, string>
{
    public bool HandlerCalled { get; private set; } = false;
    public string RequestResponse { get; } = $"response from {nameof(TestMessageHandler)}";

    public Task Handle(TestEvent eventData, CancellationToken cancellationToken)
    {
        HandlerCalled = true;
        return Task.CompletedTask;
    }

    public Task Handle(TestRequest requestData, CancellationToken cancellationToken)
    {
        HandlerCalled = true;
        return Task.CompletedTask;
    }

    public Task<string> Handle(TestRequestWithResponse request, CancellationToken cancellationToken)
    {
        HandlerCalled = true;
        return Task.FromResult(RequestResponse);
    }
}

public record OtherTestMessageHandler()
    : IEventHandler<TestEvent>
    , IRequestHandler<TestRequest>
    , IRequestHandler<TestRequestWithResponse, string>
{
    public string RequestResponse { get; } = $"response from {nameof(OtherTestMessageHandler)}";

    public Task Handle(TestEvent eventData, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task Handle(TestRequest requestData, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task<string> Handle(TestRequestWithResponse request, CancellationToken cancellationToken) => Task.FromResult(RequestResponse);
}

public record YetAnotherTestMessageHandler()
    : IEventHandler<TestEvent>
    , IRequestHandler<TestRequest>
    , IRequestHandler<TestRequestWithResponse, string>
{
    public string RequestResponse { get; } = $"response from {nameof(YetAnotherTestMessageHandler)}";

    public Task Handle(TestEvent eventData, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task Handle(TestRequest requestData, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task<string> Handle(TestRequestWithResponse request, CancellationToken cancellationToken) => Task.FromResult(RequestResponse);
}

public record OtherMessageHandler()
    : IEventHandler<OtherEvent>
    , IRequestHandler<OtherRequest>
    , IRequestHandler<OtherRequestWithResponse, object>
{
    public object RequestResponse { get; } = new();

    public Task Handle(OtherEvent eventData, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task Handle(OtherRequest requestData, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task<object> Handle(OtherRequestWithResponse request, CancellationToken cancellationToken) => Task.FromResult(RequestResponse);
}
