using Tycho.Messaging.Payload;

namespace IntegrationTests.ForwardingMessagesHorizontally.SUT.Submodules.Alpha;

internal record AlphaOutEvent(TestResult Result) : IEvent;

internal record AlphaOutRequest(TestResult Result) : IRequest;

internal record AlphaOutRequestWithResponse(TestResult Result) : IRequest<AlphaOutRequestWithResponse.Response>
{
    public record Response(string Value);
};
