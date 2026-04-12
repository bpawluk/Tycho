using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Events.Broker;
using Tycho.Modules;
using Tycho.Requests.Broker;

namespace Tycho.UnitTests.Modules;

public class TychoModuleTests
{
    [Fact]
    public void GetSettings_WhenSettingsNotProvided_ReturnsNewInstance()
    {
        // Arrange
        var sut = new ExposedSettingsModule();

        // Act
        var result = sut.GetSettingsPublic();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.Value);
    }

    [Fact]
    public void GetSettings_WhenSettingsProvided_ReturnsProvidedSettings()
    {
        // Arrange
        var expected = new TestSettings { Value = "expected" };
        var sut = new ExposedSettingsModule();
        sut.WithSettingsPublic(expected);

        // Act
        var result = sut.GetSettingsPublic();

        // Assert
        Assert.Same(expected, result);
    }

    [Fact]
    public async Task RunAsync_WithoutSourceGeneration_ThrowsNotImplementedException()
    {
        // Arrange
        var sut = new NoAutoSetupModule();

        // Act 
        async Task Act() => await sut.RunAsync();

        // Assert
        await Assert.ThrowsAsync<NotImplementedException>(Act);
    }

    private sealed class TestSettings : IModuleSettings
    {
        public string Value { get; init; } = string.Empty;
    }

    private sealed class NoAutoSetupModule : TychoModule
    {
        protected override void DefineContract(IModuleContract module) { }
        protected override void DefineEvents(IModuleEvents module) { }
        protected override void IncludeModules(IModuleStructure module) { }
        protected override void RegisterServices(IServiceCollection services) { }

        public NoAutoSetupModule()
        {
            FulfillContract(new Mock<IRequestBroker>().Object);
            PassEventBroker(new Mock<IEventBroker>().Object);
        }
    }

    private sealed class ExposedSettingsModule : TychoModule
    {
        protected override void DefineContract(IModuleContract module) { }
        protected override void DefineEvents(IModuleEvents module) { }
        protected override void IncludeModules(IModuleStructure module) { }
        protected override void RegisterServices(IServiceCollection services) { }
        protected override void __AutoSetup__(IServiceCollection services) { }

        public TychoModule WithSettingsPublic(IModuleSettings settings) => WithSettings(settings);

        public TestSettings GetSettingsPublic() => GetSettings<TestSettings>();
    }
}
