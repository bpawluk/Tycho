using Microsoft.Extensions.DependencyInjection;
using Tycho.Requests;

namespace Tycho.UnitTests.Requests;

public sealed class ServiceCollectionExtensionsTests
{
    [Fact]
    public async Task AddRequestInterceptor_WithValidOpenGeneric_ResolvesAndExecutes()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddRequestInterceptor(typeof(Interceptor<,>));
        using ServiceProvider provider = services.BuildServiceProvider();
        using IServiceScope scope = provider.CreateScope();
        IRequestInterceptor<Command, string> interceptor = scope.ServiceProvider.GetRequiredService<IRequestInterceptor<Command, string>>();
        string result = await interceptor.InterceptAsync((_, _) => Task.FromResult("handled"), new Command(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("handled", result);
    }

    [Theory]
    [InlineData(typeof(Interceptor<Command, string>))]
    [InlineData(typeof(AbstractInterceptor<,>))]
    [InlineData(typeof(OneParameterInterceptor<>))]
    [InlineData(typeof(ReversedInterceptor<,>))]
    [InlineData(typeof(Unrelated<,>))]
    public void AddRequestInterceptor_WithInvalidShape_RejectsWithoutChangingServices(Type interceptorType)
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => services.AddRequestInterceptor(interceptorType));
        Assert.Equal("interceptorType", exception.ParamName);
        Assert.Empty(services);
    }


    public class Interceptor<TRequest, TResponse> : IRequestInterceptor<TRequest, TResponse> where TRequest : class
    {
        public Task<TResponse> InterceptAsync(
            RequestHandlerDelegate<TRequest, TResponse> next,
            TRequest requestData,
            CancellationToken cancellationToken) => next(requestData, cancellationToken);
    }

    public abstract class AbstractInterceptor<TRequest, TResponse> : Interceptor<TRequest, TResponse> where TRequest : class { }
    public sealed class OneParameterInterceptor<TRequest> : Interceptor<TRequest, string> where TRequest : class { }
    public sealed class ReversedInterceptor<TResponse, TRequest> : Interceptor<TRequest, TResponse> where TRequest : class { }
    public sealed class Unrelated<TRequest, TResponse> { }
    public sealed class Command : IRequest { }
}
