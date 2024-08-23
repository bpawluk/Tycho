using Tycho.Messaging.Payload;

namespace IntegrationTests.SendingMessagesHorizontally.SUT.Submodules.Gamma;

internal record GammaInEvent(TestResult Result) : IEvent;

internal record GammaInRequest(TestResult Result) : IRequest;

internal record GammaInRequestWithResponse(TestResult Result) : IRequest<GammaInRequestWithResponse.Response>
{
    public record Response(string Value);
};
