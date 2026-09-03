using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tycho.IntegrationTests.SettingUpForHostApps.SUT;

namespace Tycho.IntegrationTests.SettingUpForHostApps;

public class SettingUpForHostAppsTests
{
    [Fact(Timeout = 5000)]
    public async Task TychoEnables_SettingItUpForHostApps()
    {
        //  Arrange
        var hostapplicationBuilder = new HostApplicationBuilder();

        string expectedResponse = "Hello World!";
        hostapplicationBuilder.Configuration["Response"] = expectedResponse;

        hostapplicationBuilder.AddTestApp(new());
        IHost host = hostapplicationBuilder.Build();

        // Act
        ITestApp app = host.Services.GetRequiredService<ITestApp>();
        string response = await app.ExecuteAsync(new TestRequest(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expectedResponse, response);

        // Dispose
        host.Dispose();
    }
}
