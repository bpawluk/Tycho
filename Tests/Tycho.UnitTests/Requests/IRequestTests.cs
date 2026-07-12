using Tycho.Requests;

namespace Tycho.UnitTests.Requests;

public class IRequestTests
{
    [Fact]
    public void RequestWithoutResponse_DoesNotImplementRequestWithUnitResponse()
    {
        Assert.False(typeof(IRequest<NoResponse>).IsAssignableFrom(typeof(IRequest)));
    }
}
