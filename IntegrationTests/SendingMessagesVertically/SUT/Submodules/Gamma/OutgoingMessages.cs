using Tycho.Messaging.Payload;

namespace IntegrationTests.SendingMessagesVertically.SUT.Submodules.Gamma;

internal record GammaOutEvent(TestResult Result) : IEvent;

internal record GammaOutRequest(TestResult Result) : IRequest;

internal record GammaOutRequestWithResponse(TestResult Result) : IRequest<GammaOutRequestWithResponse.Response>
{
    public record Response(string Value);
};
