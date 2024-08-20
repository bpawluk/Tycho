using System;
using System.Linq;
using System.Threading.Tasks;
using Tycho.Contract.Outbox;
using Tycho.Structure.Builders;
using UnitTests.Utils;

namespace UnitTests.Structure;

public class SubstructureBuilderTests 
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly SubstructureBuilder _substructureBuilder;

    public SubstructureBuilderTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _substructureBuilder = new SubstructureBuilder(_serviceProviderMock.Object);
    }

    [Fact]
    public async Task SubstructureBuilder_BuildsAllAddedSubmodules()
    {
        // Arrange
        var contractFullfilmentCalled = false;
        void contractFullfilment(IOutboxConsumer _) => contractFullfilmentCalled = true;

        _substructureBuilder.AddSubmodule<TestModule>(contractFullfilment);
        _substructureBuilder.AddSubmodule<OtherModule>();
        _substructureBuilder.AddSubmodule<YetAnotherModule>();

        // Act
        var result = await _substructureBuilder.Build();

        // Assert 
        Assert.True(contractFullfilmentCalled);
        Assert.Equal(3, result.Count());
        Assert.Single(result.Where(module => typeof(TestModule).Equals(module.GetType().GetGenericArguments()[0])));
        Assert.Single(result.Where(module => typeof(OtherModule).Equals(module.GetType().GetGenericArguments()[0])));
        Assert.Single(result.Where(module => typeof(YetAnotherModule).Equals(module.GetType().GetGenericArguments()[0])));
    }
}
