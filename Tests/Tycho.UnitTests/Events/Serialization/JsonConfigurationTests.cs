using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.Events.Serialization;

namespace Tycho.UnitTests.Events.Serialization;

public sealed class JsonConfigurationTests
{
    [Fact]
    public void Configure_RoundTripsNamingPolicyAndConverterWithoutChangingOtherJsonOptions()
    {
        // Arrange
        var unrelated = new JsonSerializerOptions();
        var services = new ServiceCollection();
        services.AddSingleton(unrelated);
        services.AddTransient<IPayloadSerializer, JsonPayloadSerializer>();
        JsonSerializerOptions? captured = null;

        // Act
        services.ConfigureTychoEventProcessing(settings =>
        {
            JsonSerializerOptions options = settings.PayloadSerializer.JsonOptions;
            captured = options;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.Converters.Add(new JsonStringEnumConverter());
        });
        captured!.PropertyNamingPolicy = null;
        using ServiceProvider provider = services.BuildServiceProvider();
        IPayloadSerializer serializer = provider.GetRequiredService<IPayloadSerializer>();
        string payload = serializer.Serialize(new Payload { EventName = "hello", State = State.Ready });
        Payload restored = serializer.Deserialize<Payload>(payload);

        // Assert
        Assert.Equal("{\"eventName\":\"hello\",\"state\":\"Ready\"}", payload);
        Assert.Equal("hello", restored.EventName);
        Assert.Equal(State.Ready, restored.State);
        Assert.Same(unrelated, provider.GetRequiredService<JsonSerializerOptions>());
        Assert.Null(unrelated.PropertyNamingPolicy);
        Assert.Empty(unrelated.Converters);
        Assert.NotSame(serializer, provider.GetRequiredService<IPayloadSerializer>());
        Assert.Equal(payload, provider.GetRequiredService<IPayloadSerializer>().Serialize(restored));
    }

    [Fact]
    public void Configure_ReplacesPreviousOptionsStartingFromDefaults()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IPayloadSerializer, JsonPayloadSerializer>();
        services.ConfigureTychoEventProcessing(options => options.PayloadSerializer.JsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

        // Act
        IServiceCollection result = services.ConfigureTychoEventProcessing(_ => { });
        using ServiceProvider provider = services.BuildServiceProvider();

        // Assert
        Assert.Same(services, result);
        Assert.Single(provider.GetServices<JsonPayloadSerializerSettings>());
        Assert.Contains("\"EventName\"", provider.GetRequiredService<IPayloadSerializer>().Serialize(new Payload()));
    }

    [Fact]
    public void Configure_RejectsNullArgumentsAndCallbackFailureLeavesRegistrationsUnchanged()
    {
        // Arrange
        var services = new ServiceCollection();
        services.ConfigureTychoEventProcessing(_ => { });
        ServiceDescriptor[] original = [.. services];

        // Act
        void ConfigureWithNullCallback() => services.ConfigureTychoEventProcessing(null!);
        void ConfigureWithNullServices() => Tycho.ServiceCollectionExtensions.ConfigureTychoEventProcessing(null!, _ => { });
        void ConfigureWithFailingCallback() => services.ConfigureTychoEventProcessing(_ => throw new InvalidOperationException());

        // Assert
        Assert.Throws<ArgumentNullException>(ConfigureWithNullCallback);
        Assert.Throws<ArgumentNullException>(ConfigureWithNullServices);
        Assert.Throws<InvalidOperationException>(ConfigureWithFailingCallback);
        Assert.Equal(original, [.. services]);
    }

    public sealed class Payload : IEvent
    {
        public string EventName { get; set; } = "";
        public State State { get; set; }
    }

    public enum State { Ready }
}
