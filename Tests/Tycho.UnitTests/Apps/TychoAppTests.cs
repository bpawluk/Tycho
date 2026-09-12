using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tycho.Apps;
using Tycho.Apps.Instance;
using Tycho.Hosting;
using Tycho.Hosting.Files;

namespace Tycho.UnitTests.Apps;

public class TychoAppTests
{
    [Fact]
    public void DefineContract_GetsCalledWithContractWhenBuildingTheApp()
    {
        // Arrange
        var appDefinition = new TestApp();

        // Act
        using IApp app = appDefinition.CreateAppBuilderBase().Build(default);

        // Assert
        Assert.NotNull(appDefinition.Contract);
    }

    [Fact]
    public void DefineEvents_GetsCalledWithEventsWhenBuildingTheApp()
    {
        // Arrange
        var appDefinition = new TestApp();

        // Act
        using IApp app = appDefinition.CreateAppBuilderBase().Build(default);

        // Assert
        Assert.NotNull(appDefinition.Events);
    }

    [Fact]
    public void IncludeModules_GetsCalledWithStructureWhenBuildingTheApp()
    {
        // Arrange
        var appDefinition = new TestApp();

        // Act
        using IApp app = appDefinition.CreateAppBuilderBase().Build(default);

        // Assert
        Assert.NotNull(appDefinition.Structure);
    }

    [Fact]
    public void RegisterServices_GetsCalledWithHostServiceCollectionWhenBuildingTheApp()
    {
        // Arrange
        var appDefinition = new TestApp();

        // Act
        using IApp app = appDefinition.CreateAppBuilderBase().Build(default);

        // Assert
        Assert.NotNull(appDefinition.Services);
        Assert.NotNull(app.Internals.GetService<TestService>());
    }

    [Fact]
    public void ConfigureHost_WithParentServiceProvider_InheritsHostConfigurtion()
    {
        // Arrange
        var appDefinition = new TestApp();
        string expectedAppName = appDefinition.GetType().Assembly.GetName().Name!;

        const string parentEnvironmentName = "parentEnv";
        string parentContentRootPath = Environment.CurrentDirectory;
        HostApplicationBuilder parentBuilder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings
        {
            ApplicationName = "parentApp",
            EnvironmentName = parentEnvironmentName,
            ContentRootPath = parentContentRootPath,
        });

        const string parentSettingKey = "key";
        const string parentSettingValue = "value";
        parentBuilder.Configuration[parentSettingKey] = parentSettingValue;

        using IHost parentHost = parentBuilder.Build();

        // Act
        using IApp app = appDefinition.CreateAppBuilderBase().Build(parentHost.Services);

        // Assert
        Assert.IsType<StandaloneHostLifetime>(app.Internals.GetRequiredService<IHostLifetime>());

        IHostEnvironment environment = app.Internals.GetRequiredService<IHostEnvironment>();
        Assert.Equal(expectedAppName, environment.ApplicationName);
        Assert.Equal(parentEnvironmentName, environment.EnvironmentName);
        Assert.Equal(parentContentRootPath, environment.ContentRootPath);
        Assert.IsType<NonDisposingFileProvider>(environment.ContentRootFileProvider);

        IConfiguration configuration = app.Internals.GetRequiredService<IConfiguration>();
        Assert.Equal(parentSettingValue, configuration[parentSettingKey]);
        Assert.Equal(expectedAppName, configuration[HostDefaults.ApplicationKey]);
        Assert.Equal(parentEnvironmentName, configuration[HostDefaults.EnvironmentKey]);
        Assert.Equal(parentContentRootPath, configuration[HostDefaults.ContentRootKey]);

        ILoggerFactory parentLoggerFactory = parentHost.Services.GetRequiredService<ILoggerFactory>();
        Assert.Same(parentLoggerFactory, app.Internals.GetRequiredService<ILoggerFactory>());
    }

    [Fact]
    public void ConfigureHost_WithoutParentServiceProvider_ConfiguresDefaultHost()
    {
        // Arrange
        var appDefinition = new TestApp();
        string expectedAppName = appDefinition.GetType().Assembly.GetName().Name!;

        // Act
        using IApp app = appDefinition.CreateAppBuilderBase().Build(default);

        // Assert
        Assert.IsType<StandaloneHostLifetime>(app.Internals.GetRequiredService<IHostLifetime>());

        IHostEnvironment environment = app.Internals.GetRequiredService<IHostEnvironment>();
        Assert.Equal(expectedAppName, environment.ApplicationName);

        IConfiguration configuration = app.Internals.GetRequiredService<IConfiguration>();
        Assert.Equal(expectedAppName, configuration[HostDefaults.ApplicationKey]);
    }

    [Fact]
    public async Task StartupAndCleanup_GetCalledWhenStartingAndStoppingTheApp()
    {
        // Arrange
        var appDefinition = new TestApp();
        var cancellationToken = new CancellationToken();

        using IApp app = appDefinition.CreateAppBuilderBase().Build(default);

        // Act & Assert
        Assert.Equal(0, appDefinition.StartupCalls);
        await app.StartAsync(cancellationToken);
        Assert.Equal(1, appDefinition.StartupCalls);

        Assert.Equal(0, appDefinition.CleanupCalls);
        await app.StopAsync(cancellationToken);
        Assert.Equal(1, appDefinition.CleanupCalls);
    }

    private sealed class TestApp : TychoApp
    {
        public IAppContract? Contract { get; private set; }
        public IAppEvents? Events { get; private set; }
        public IAppStructure? Structure { get; private set; }
        public IServiceCollection? Services { get; private set; }
        public int StartupCalls { get; private set; }
        public int CleanupCalls { get; private set; }

        protected override void DefineContract(IAppContract app) => Contract = app;
        protected override void DefineEvents(IAppEvents app) => Events = app;
        protected override void IncludeModules(IAppStructure app) => Structure = app;
        protected override void RegisterServices(IServiceCollection services)
        {
            Services = services;
            services.AddSingleton<TestService>();
        }

        protected override Task Startup(IServiceProvider app, CancellationToken cancellationToken)
        {
            StartupCalls++;
            return Task.CompletedTask;
        }

        protected override Task Cleanup(IServiceProvider app, CancellationToken cancellationToken)
        {
            CleanupCalls++;
            return Task.CompletedTask;
        }
    }

    private sealed class TestAppSetup
    {
        public static void Setup(IServiceCollection app) { }
    }

    private sealed class TestService { }
}
