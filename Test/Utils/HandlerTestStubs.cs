using System.Threading.Tasks;
using System.Threading;
using Tycho.Messaging.Handlers;

namespace Test.Utils;

public record TestMessageHandler()
    : IEventHandler<TestEvent>
    , ICommandHandler<TestCommand>
    , IQueryHandler<TestQuery, string>
{
    public string QueryResponse { get; } = $"response from {nameof(TestMessageHandler)}";

    public Task Handle(TestEvent eventData, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task Handle(TestCommand commandData, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task<string> Handle(TestQuery query, CancellationToken cancellationToken) => Task.FromResult(QueryResponse);
}

public record OtherTestMessageHandler()
    : IEventHandler<TestEvent>
    , ICommandHandler<TestCommand>
    , IQueryHandler<TestQuery, string>
{
    public string QueryResponse { get; } = $"response from {nameof(OtherTestMessageHandler)}";

    public Task Handle(TestEvent eventData, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task Handle(TestCommand commandData, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task<string> Handle(TestQuery query, CancellationToken cancellationToken) => Task.FromResult(QueryResponse);
}

public record YetAnotherTestMessageHandler()
    : IEventHandler<TestEvent>
    , ICommandHandler<TestCommand>
    , IQueryHandler<TestQuery, string>
{
    public string QueryResponse { get; } = $"response from {nameof(YetAnotherTestMessageHandler)}";

    public Task Handle(TestEvent eventData, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task Handle(TestCommand commandData, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task<string> Handle(TestQuery query, CancellationToken cancellationToken) => Task.FromResult(QueryResponse);
}

public record OtherMessageHandler()
    : IEventHandler<OtherEvent>
    , ICommandHandler<OtherCommand>
    , IQueryHandler<OtherQuery, object>
{
    public object QueryResponse { get; } = new();

    public Task Handle(OtherEvent eventData, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task Handle(OtherCommand commandData, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task<object> Handle(OtherQuery query, CancellationToken cancellationToken) => Task.FromResult(QueryResponse);
}
