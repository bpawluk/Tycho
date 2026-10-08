using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Tycho.Modules;
using Tycho.Modules.Instance;
using Tycho.Modules.Setup;
using Tycho.Requests;
using Tycho.Requests.Broker;
using Tycho.Structure;
using Tycho.UnitTests._Data.Modules;

namespace Tycho.UnitTests.Modules.Setup;

public sealed class ContractFulfillmentTests : IDisposable
{
    private readonly Internals _internals = new(Host.CreateEmptyApplicationBuilder(null), typeof(TestModule));
    private readonly Mock<IRequestBroker> _destination = new(MockBehavior.Strict);
    private readonly ContractFulfillment<TestModule> _sut;
    private readonly DownStreamBroker<TestModule> _broker;

    public ContractFulfillmentTests()
    {
        _internals.GetHostBuilder().Services
            .AddSingleton(_internals)
            .AddSingleton(Mock.Of<IModule<OtherModule>>(module => module.RequestBroker == _destination.Object));
        _sut = new ContractFulfillment<TestModule>(_internals);
        _broker = new DownStreamBroker<TestModule>(_internals);
    }

    [Fact]
    public async Task ForwardsTo_WithResponse_PreservesTokenAndMapsRequestAndResponseOnce()
    {
        // Arrange
        var request = new SourceQuery("input");
        var mapped = new TargetQuery("mapped:input");
        CancellationToken token = TestContext.Current.CancellationToken;
        int requestMappingCalls = 0;
        int responseMappingCalls = 0;
        _destination.Setup(broker => broker.ExecuteAsync<TargetQuery, int>(mapped, token)).ReturnsAsync(42);
        IContractFulfillment root = _sut.Fulfills<SourceQuery, string>()
            .MapsTo<TargetQuery, int>(
                source =>
                {
                    requestMappingCalls++;
                    return new TargetQuery("mapped:" + source.Value);
                },
                response =>
                {
                    responseMappingCalls++;
                    return "response:" + response;
                })
            .ForwardsTo<OtherModule>();
        _internals.Build();

        // Act
        string response = await _broker.ExecuteAsync<SourceQuery, string>(request, token);

        // Assert
        Assert.Equal("response:42", response);
        Assert.Same(_sut, root);
        Assert.Equal(1, requestMappingCalls);
        Assert.Equal(1, responseMappingCalls);
        _destination.Verify(broker => broker.ExecuteAsync<TargetQuery, int>(mapped, token), Times.Once);
        _destination.VerifyNoOtherCalls();
    }

    public void Dispose() => _internals.Dispose();

    public sealed record SourceQuery(string Value) : IRequest<string>;
    public sealed record TargetQuery(string Value) : IRequest<int>;
}
