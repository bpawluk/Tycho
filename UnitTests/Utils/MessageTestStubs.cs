using Tycho.Messaging.Payload;

namespace UnitTests.Utils;

public record TestEvent(string Name) : IEvent;
public record OtherEvent(int Age) : IEvent;

public record TestRequest(string Name) : IRequest;
public record OtherRequest(int Age) : IRequest;

public record TestRequestWithResponse(string Name) : IRequest<string>;
public record OtherTestRequestWithResponse(string Name) : IRequest<string>;
public record OtherRequestWithResponse(int Age) : IRequest<object>;