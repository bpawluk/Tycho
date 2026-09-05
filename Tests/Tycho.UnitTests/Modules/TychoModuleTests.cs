using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Tycho.Events.Broker;
using Tycho.Hosting;
using Tycho.Hosting.Files;
using Tycho.Modules;
using Tycho.Modules.Instance;
using Tycho.Requests.Broker;

namespace Tycho.UnitTests.Modules;

public class TychoModuleTests
{
    [Fact]
    public void DefineContract_GetsCalledWithContractWhenBuildingTheModule()
    {
        // Arrange
        var moduleDefinition = new TestModule();

        // Act
        using IModule module = BuildModule(moduleDefinition);

        // Assert
        Assert.NotNull(moduleDefinition.Contract);
    }

    [Fact]
    public void DefineEvents_GetsCalledWithEventsWhenBuildingTheModule()
    {
        // Arrange
        var moduleDefinition = new TestModule();

        // Act
        using IModule module = BuildModule(moduleDefinition);

        // Assert
        Assert.NotNull(moduleDefinition.Events);
    }

    [Fact]
    public void IncludeModules_GetsCalledWithStructureWhenBuildingTheModule()
    {
        // Arrange
        var moduleDefinition = new TestModule();

        // Act
        using IModule module = BuildModule(moduleDefinition);

        // Assert
        Assert.NotNull(moduleDefinition.Structure);
    }

    [Fact]
    public void RegisterServices_GetsCalledWithHostServiceCollectionWhenBuildingTheModule()
    {
        // Arrange
        var moduleDefinition = new TestModule();

        // Act
        using IModule module = BuildModule(moduleDefinition);

        // Assert
        Assert.NotNull(moduleDefinition.Services);
        Assert.NotNull(module.Internals.GetService<TestService>());
    }

    [Fact]
    public void ConfigureHost_WithParentServiceProvider_InheritsHostConfiguration()
    {
        // Arrange
        var moduleDefinition = new TestModule();
        string expectedModuleName = moduleDefinition.GetType().Assembly.GetName().Name!;

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
        using IModule module = BuildModule(moduleDefinition, parentHost.Services);

        // Assert
        Assert.IsType<StandaloneHostLifetime>(module.Internals.GetRequiredService<IHostLifetime>());

        IHostEnvironment environment = module.Internals.GetRequiredService<IHostEnvironment>();
        Assert.Equal(expectedModuleName, environment.ApplicationName);
        Assert.Equal(parentEnvironmentName, environment.EnvironmentName);
        Assert.Equal(parentContentRootPath, environment.ContentRootPath);
        Assert.IsType<NonDisposingFileProvider>(environment.ContentRootFileProvider);

        IConfiguration configuration = module.Internals.GetRequiredService<IConfiguration>();
        Assert.Equal(parentSettingValue, configuration[parentSettingKey]);
        Assert.Equal(expectedModuleName, configuration[HostDefaults.ApplicationKey]);
        Assert.Equal(parentEnvironmentName, configuration[HostDefaults.EnvironmentKey]);
        Assert.Equal(parentContentRootPath, configuration[HostDefaults.ContentRootKey]);

        ILoggerFactory parentLoggerFactory = parentHost.Services.GetRequiredService<ILoggerFactory>();
        Assert.Same(parentLoggerFactory, module.Internals.GetRequiredService<ILoggerFactory>());
    }

    [Fact]
    public void ConfigureHost_WithoutParentServiceProvider_ConfiguresDefaultHost()
    {
        // Arrange
        var moduleDefinition = new TestModule();
        string expectedModuleName = moduleDefinition.GetType().Assembly.GetName().Name!;

        // Act
        using IModule module = BuildModule(moduleDefinition);

        // Assert
        Assert.IsType<StandaloneHostLifetime>(module.Internals.GetRequiredService<IHostLifetime>());

        IHostEnvironment environment = module.Internals.GetRequiredService<IHostEnvironment>();
        Assert.Equal(expectedModuleName, environment.ApplicationName);

        IConfiguration configuration = module.Internals.GetRequiredService<IConfiguration>();
        Assert.Equal(expectedModuleName, configuration[HostDefaults.ApplicationKey]);
    }

    [Fact]
    public async Task StartupAndCleanup_GetCalledWhenStartingAndStoppingTheModule()
    {
        // Arrange
        var moduleDefinition = new TestModule();
        var cancellationToken = new CancellationToken();

        // Act & Assert
        using IModule module = BuildModule(moduleDefinition);

        Assert.Equal(0, moduleDefinition.StartupCalls);
        await module.StartAsync(cancellationToken);
        Assert.Equal(1, moduleDefinition.StartupCalls);

        Assert.Equal(0, moduleDefinition.CleanupCalls);
        await module.StopAsync(cancellationToken);
        Assert.Equal(1, moduleDefinition.CleanupCalls);
    }

    [Fact]
    public void GetSettings_WhenSettingsNotProvided_ReturnsNewInstance()
    {
        // Arrange
        var moduleDefinition = new TestModule();

        // Act
        TestSettings result = moduleDefinition.GetSettings();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.Value);
    }

    [Fact]
    public void GetSettings_WhenSettingsProvided_ReturnsProvidedSettings()
    {
        // Arrange
        var moduleDefinition = new TestModule();
        var expected = new TestSettings { Value = "expected" };
        moduleDefinition.WithSettings(expected);

        // Act
        TestSettings result = moduleDefinition.GetSettings();

        // Assert
        Assert.Same(expected, result);
    }

    private static IModule BuildModule(TestModule moduleDefinition, IServiceProvider? parentServiceProvider = null)
    {
        moduleDefinition.FulfillContract(new Mock<IRequestBroker>().Object);
        moduleDefinition.PassEventBroker(new Mock<IEventBroker>().Object);
        return moduleDefinition.CreateModuleBuilder().Build(parentServiceProvider);
    }

    private sealed class TestSettings : IModuleSettings
    {
        public string Value { get; init; } = string.Empty;
    }

    private sealed class TestModule : TychoModule
    {
        public IModuleContract? Contract { get; private set; }
        public IModuleEvents? Events { get; private set; }
        public IModuleStructure? Structure { get; private set; }
        public IServiceCollection? Services { get; private set; }
        public int StartupCalls { get; private set; }
        public int CleanupCalls { get; private set; }

        protected override void DefineContract(IModuleContract module) => Contract = module;
        protected override void DefineEvents(IModuleEvents module) => Events = module;
        protected override void IncludeModules(IModuleStructure module) => Structure = module;
        protected override void RegisterServices(IServiceCollection services)
        {
            Services = services;
            services.AddSingleton<TestService>();
        }

        protected override Task Startup(IServiceProvider module, CancellationToken cancellationToken)
        {
            StartupCalls++;
            return Task.CompletedTask;
        }

        protected override Task Cleanup(IServiceProvider module, CancellationToken cancellationToken)
        {
            CleanupCalls++;
            return Task.CompletedTask;
        }

        public new TychoModule WithSettings(IModuleSettings settings) => base.WithSettings(settings);

        public TestSettings GetSettings() => GetSettings<TestSettings>();
    }

    private sealed class TestModuleSetup
    {
        public static void Setup(IServiceCollection module) { }
    }

    private sealed class TestService { }
}
