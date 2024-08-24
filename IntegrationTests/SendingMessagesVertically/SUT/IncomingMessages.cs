using Tycho.Messaging.Payload;

namespace IntegrationTests.SendingMessagesVertically.SUT;

internal record EventToSend(TestResult Result) : IEvent;

internal record EventToSendWithMapping(TestResult Result) : IEvent;

internal record RequestToSend(TestResult Result) : IRequest;

internal record RequestToSendWithMapping(TestResult Result) : IRequest;

internal record RequestWithResponseToSend(TestResult Result) : IRequest<string>;

internal record RequestWithResponseToSendWithMapping(TestResult Result) : IRequest<string>;