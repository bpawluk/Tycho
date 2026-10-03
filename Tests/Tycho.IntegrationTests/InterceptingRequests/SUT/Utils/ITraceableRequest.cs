namespace Tycho.IntegrationTests.InterceptingRequests.SUT.Utils;

public interface ITraceableRequest
{
    List<string> Trace { get; }
}
