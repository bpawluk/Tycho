using Tycho.Messaging.Payload;

namespace IntegrationTests.ForwardingMessagesHorizontally.SUT.Submodules.Beta;

internal record BetaOutEvent(TestResult Result) : IEvent;

internal record BetaOutRequest(TestResult Result) : IRequest;

internal record BetaOutRequestWithResponse(TestResult Result) : IRequest<BetaOutRequestWithResponse.Response>
{
    public record Response(string Value);
};
