using Tycho.Messaging.Payload;

namespace IntegrationTests.ForwardingMessagesHorizontally.SUT.Submodules.Alpha;

internal record AlphaInEvent(TestResult Result) : IEvent;

internal record AlphaInRequest(TestResult Result) : IRequest;

internal record AlphaInRequestWithResponse(TestResult Result) : IRequest<AlphaInRequestWithResponse.Response>
{
    public record Response(string Value);
};
