using Tycho.Messaging.Payload;

namespace UnitTests.Utils;

public record TestEvent(string Name) : IEvent;
public record OtherEvent(int Age) : IEvent;

public record TestCommand(string Name) : IRequest;
public record OtherCommand(int Age) : IRequest;

public record TestQuery(string Name) : IRequest<string>;
public record OtherTestQuery(string Name) : IRequest<string>;
public record OtherQuery(int Age) : IRequest<object>;