using Tycho.Messaging.Payload;

namespace IntegrationTests.ForwardingMessagesHorizontally.SUT;

internal record EventToForward(TestResult Result) : IEvent;

internal record EventToForwardWithMapping(TestResult Result) : IEvent;

internal record RequestToForward(TestResult Result) : IRequest;

internal record RequestToForwardWithMapping(TestResult Result) : IRequest;

internal record RequestWithResponseToForward(TestResult Result) : IRequest<string>;

internal record RequestWithResponseToForwardWithMapping(TestResult Result) : IRequest<string>;