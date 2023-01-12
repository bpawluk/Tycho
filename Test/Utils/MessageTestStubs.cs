using Tycho.Messaging.Payload;

namespace Test.Utils;

public record TestEvent(string Name) : IEvent;
public record OtherEvent(int Age) : IEvent;

public record TestCommand(string Name) : ICommand;
public record OtherCommand(int Age) : ICommand;

public record TestQuery(string Name) : IQuery<string>;
public record OtherQuery(int Age) : IQuery<object>;