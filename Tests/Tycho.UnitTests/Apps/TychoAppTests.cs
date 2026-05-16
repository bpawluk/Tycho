using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Tycho.Apps;
using Tycho.Apps.Instance;
using Tycho.Events.Serialization;

namespace Tycho.UnitTests.Apps;

public class TychoAppTests
{
    [Fact]
    public void WithConfiguration_NullConfiguration_ThrowsArgumentNullException()
    {
        // Arrange
        var sut = new ExposedInternalsApp();

        // Act
        void Act() => sut.CallWithConfigurationBase(null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(Act);
    }

    [Fact]
    public void WithLogging_NullLoggingSetup_ThrowsArgumentNullException()
    {
        // Arrange
        var sut = new ExposedInternalsApp();

        // Act
        void Act() => sut.CallWithLoggingBase(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Act);
    }

    [Fact]
    public async Task RunAsync_CalledTwice_ThrowsInvalidOperationException()
    {
        // Arrange
        var sut = new ExposedInternalsApp();
        await sut.RunAsync();

        // Act
        async Task Act() => await sut.RunAsync();

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
    }

    [Fact]
    public async Task RunAsync_CalledTwiceConcurrently_OnlyFirstSucceeds()
    {
        // Arrange
        var sut = new ExposedInternalsApp();

        // Act
        var tasks = Enumerable.Range(0, 2).Select((_) =>
        {
            return sut.RunAsync().ContinueWith(t => t.IsFaulted ? t.Exception!.InnerException : null);
        });
        var results = await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(2, results.Length);
        Assert.Contains(results, r => r == null);
        Assert.Contains(results, r => r is InvalidOperationException);
    }

    [Fact]
    public async Task RunAsync_WithoutSourceGeneration_ThrowsNotImplementedException()
    {
        // Arrange
        var sut = new NoAutoSetupApp();

        // Act
        async Task Act() => await sut.RunAsync();

        // Assert
        await Assert.ThrowsAsync<NotImplementedException>(Act);
    }

    private sealed class NoAutoSetupApp : TychoApp
    {
        protected override void DefineContract(IAppContract app) { }
        protected override void DefineEvents(IAppEvents app) { }
        protected override void IncludeModules(IAppStructure app) { }
        protected override void RegisterServices(IServiceCollection app)
        {
            var eventSerializerMock = new Mock<IEventSerializer>();
            app.AddSingleton(eventSerializerMock.Object);
        }
        public Task<IApp> RunAsync() => RunBaseAsync();
    }

    internal class ExposedInternalsApp : TychoApp
    {
        protected override void DefineContract(IAppContract app) { }

        protected override void DefineEvents(IAppEvents app) { }

        protected override void IncludeModules(IAppStructure app) { }

        protected override void RegisterServices(IServiceCollection app)
        {
            var eventSerializerMock = new Mock<IEventSerializer>();
            app.AddSingleton(eventSerializerMock.Object);
        }

        protected override void __AutoSetup__(IServiceCollection app) { }

        public Task<IApp> RunAsync() => RunBaseAsync();

        public void CallWithConfigurationBase(IConfiguration config) => WithConfigurationBase(config);

        public void CallWithLoggingBase(Action<ILoggingBuilder> setup) => WithLoggingBase(setup);
    }
}
