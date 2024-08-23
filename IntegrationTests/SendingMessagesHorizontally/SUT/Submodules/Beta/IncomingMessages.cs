using Tycho.Messaging.Payload;

namespace IntegrationTests.SendingMessagesHorizontally.SUT.Submodules.Beta;

internal record BetaInEvent(TestResult Result) : IEvent;

internal record BetaInRequest(TestResult Result) : IRequest;

internal record BetaInRequestWithResponse(TestResult Result) : IRequest<BetaInRequestWithResponse.Response>
{
    public record Response(string Value);
};
