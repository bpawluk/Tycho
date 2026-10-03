using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tycho.Apps;
using Tycho.Apps.Instance;
using Tycho.Apps.Setup;
using Tycho.Structure;

namespace Tycho.UnitTests.Apps.Setup;

public sealed class AppBuilderBaseTests
{
    private readonly AppBuilderBase _sut = new(typeof(TychoApp));

    [Fact]
    public void Build_WhenAlreadyBuilt_ThrowsWithoutCreatingAnotherHost()
    {
        // Arrange
        int hostCalls = 0;
        _sut.WithHostBuilder(() =>
        {
            hostCalls++;
            return Host.CreateEmptyApplicationBuilder(null);
        });
        using IApp app = _sut.Build(null);

        // Act
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => _sut.Build(null));

        // Assert
        Assert.Equal("The app has already been built.", exception.Message);
        Assert.Equal(1, hostCalls);
    }

    [Fact]
    public void Build_WithoutHostBuilder_ThrowsBeforeConfiguringApp()
    {
        // Arrange
        int configurationCalls = 0;
        _sut.WithStructure(_ => configurationCalls++);
        _sut.WithContract(_ => configurationCalls++);
        _sut.WithEvents(_ => configurationCalls++);
        _sut.WithServices(_ => configurationCalls++);
        _sut.WithHostConfiguration((_, _) => configurationCalls++);

        // Act
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => _sut.Build(null));

        // Assert
        Assert.Equal("The app host builder has not been configured.", exception.Message);
        Assert.Equal(0, configurationCalls);
    }

    [Fact]
    public void Build_WithoutOptionalConfiguration_CreatesUsableApp()
    {
        // Arrange
        _sut.WithHostBuilder(() => Host.CreateEmptyApplicationBuilder(null));

        // Act
        using IApp app = _sut.Build(null);

        // Assert
        Assert.IsAssignableFrom<IApp<TychoApp>>(app);
        Assert.Same(app.Internals, app.Internals.GetRequiredService<Internals>());
        Assert.NotNull(app.Internals.GetRequiredService<IHostEnvironment>());
        Assert.NotNull(app.RequestBroker);
    }

    [Theory]
    [InlineData("WithHostBuilder", "createHostBuilder")]
    [InlineData("WithHostConfiguration", "configureHost")]
    [InlineData("WithContract", "configureContract")]
    [InlineData("WithEvents", "configureEvents")]
    [InlineData("WithStructure", "configureStructure")]
    [InlineData("WithServices", "registerServices")]
    [InlineData("WithStartup", "startup")]
    [InlineData("WithCleanup", "cleanup")]
    public void WithConfiguration_WithNullDelegate_ThrowsArgumentNullException(string method, string parameter)
    {
        // Act
        AppBuilderBase Act() => method switch
        {
            "WithHostBuilder" => _sut.WithHostBuilder(null!),
            "WithHostConfiguration" => _sut.WithHostConfiguration(null!),
            "WithContract" => _sut.WithContract(null!),
            "WithEvents" => _sut.WithEvents(null!),
            "WithStructure" => _sut.WithStructure(null!),
            "WithServices" => _sut.WithServices(null!),
            "WithStartup" => _sut.WithStartup(null!),
            "WithCleanup" => _sut.WithCleanup(null!),
            _ => throw new ArgumentOutOfRangeException(nameof(method))
        };

        // Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => Act());
        Assert.Equal(parameter, exception.ParamName);
    }

    [Fact]
    public void WithConfiguration_WithValidDelegates_ReturnsSameBuilder()
    {
        // Act & Assert
        Assert.Same(_sut, _sut.WithHostBuilder(() => Host.CreateEmptyApplicationBuilder(null)));
        Assert.Same(_sut, _sut.WithHostConfiguration((_, _) => { }));
        Assert.Same(_sut, _sut.WithContract(_ => { }));
        Assert.Same(_sut, _sut.WithEvents(_ => { }));
        Assert.Same(_sut, _sut.WithStructure(_ => { }));
        Assert.Same(_sut, _sut.WithServices(_ => { }));
        Assert.Same(_sut, _sut.WithStartup((_, _) => Task.CompletedTask));
        Assert.Same(_sut, _sut.WithCleanup((_, _) => Task.CompletedTask));
    }
}
