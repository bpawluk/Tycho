using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tycho.IntegrationTests.SettingUpForHostApps.SUT;
using Tycho.Structure;

namespace Tycho.IntegrationTests.SettingUpForHostApps;

public class SettingUpForHostAppsTests
{
    [Fact(Timeout = 500)]
    public async Task TychoEnables_SettingItUpForHostApps()
    {
        //  Arrange
        var hostapplicationBuilder = new HostApplicationBuilder();

        var expectedResponse = "Hello World!";
        hostapplicationBuilder.Configuration["Response"] = expectedResponse;

        await hostapplicationBuilder.AddTycho<TestApp>();
        var host = hostapplicationBuilder.Build();

        // Act
        var app = host.Services.GetRequiredService<IApp>();
        var response = await app.Execute<TestRequest, string>(new());

        // Assert
        Assert.Equal(expectedResponse, response);

        // Dispose
        host.Dispose();
    }
}