using IntegrationTests.ProvidingConfiguration.SUT;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tycho;

namespace IntegrationTests.ProvidingConfiguration;

public class ProvidingConfigurationTests : IAsyncLifetime
{
    private readonly Dictionary<string, string?> _config;
    private IModule? _sut;

    public ProvidingConfigurationTests()
    {
        _config = new Dictionary<string, string?>()
        {
            ["SomeNumber"] = "100",
            ["SomeDate"] = DateTime.UtcNow.ToString(),
        };
    }

    public async Task InitializeAsync()
    {
        _sut = await new AppModule()
            .Configure(builder =>
            {
                builder.AddInMemoryCollection(_config);
            })
            .Build();
    }

    [Fact]
    public async Task Tycho_Enables_ProvidingModuleConfiguration()
    {
        // Arrange
        // - no arrangement required

        // Act
        var configuredNumber = await _sut!.Execute<GetConfiguredValueViaBindingRequest, int>(new());
        var configuredDate = await _sut.Execute<GetConfiguredValueViaIConfigurationRequest, DateTime>(new());

        // Assert
        Assert.Equal(_config["SomeNumber"], configuredNumber.ToString());
        Assert.Equal(_config["SomeDate"], configuredDate.ToString());
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
