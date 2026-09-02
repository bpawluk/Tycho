using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;

namespace Tycho.UnitTests.Modules;

public class TychoModuleTests
{
    [Fact]
    public void GetSettings_WhenSettingsNotProvided_ReturnsNewInstance()
    {
        // Arrange
        var sut = new ExposedSettingsModule();

        // Act
        TestSettings result = sut.GetSettingsPublic();

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
        TestSettings result = sut.GetSettingsPublic();

        // Assert
        Assert.Same(expected, result);
    }

    private sealed class TestSettings : IModuleSettings
    {
        public string Value { get; init; } = string.Empty;
    }

    private sealed class ExposedSettingsModule : TychoModule
    {
        protected override void DefineContract(IModuleContract module) { }
        protected override void DefineEvents(IModuleEvents module) { }
        protected override void IncludeModules(IModuleStructure module) { }
        protected override void RegisterServices(IServiceCollection services) { }

        public TychoModule WithSettingsPublic(IModuleSettings settings) => WithSettings(settings);

        public TestSettings GetSettingsPublic() => GetSettings<TestSettings>();
    }
}
