using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.Events.Inbox;
using Tycho.Events.Outbox;
using Tycho.Events.Serialization;

namespace Tycho.UnitTests.Events;

public sealed class EventProcessingSettingsTests
{
    [Fact]
    public void Configure_RegistersSeparateSettingsAndReplacesAllSettingsFromDefaults()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.ConfigureTychoEventProcessing(options =>
        {
            options.Inbox.ConcurrencyLimit = 2;
            options.Outbox.ConcurrencyLimit = 3;
            options.PayloadSerializer.JsonOptions.PropertyNameCaseInsensitive = true;
        });
        using ServiceProvider provider = services.BuildServiceProvider();
        InboxSettings[] inboxSettings = [.. provider.GetServices<InboxSettings>()];
        OutboxSettings[] outboxSettings = [.. provider.GetServices<OutboxSettings>()];
        JsonPayloadSerializerSettings[] serializerSettings = [.. provider.GetServices<JsonPayloadSerializerSettings>()];
        EventProcessingSettings? registeredOptions = provider.GetService<EventProcessingSettings>();
        IServiceCollection result = services.ConfigureTychoEventProcessing(options => options.Inbox.ConcurrencyLimit = 4);
        using ServiceProvider updated = services.BuildServiceProvider();

        // Assert
        Assert.Equal(2, Assert.Single(inboxSettings).ConcurrencyLimit);
        Assert.Equal(3, Assert.Single(outboxSettings).ConcurrencyLimit);
        Assert.True(Assert.Single(serializerSettings).JsonOptions.PropertyNameCaseInsensitive);
        Assert.Null(registeredOptions);
        Assert.Same(services, result);
        Assert.Equal(4, Assert.Single(updated.GetServices<InboxSettings>()).ConcurrencyLimit);
        Assert.Equal(new OutboxSettings().ConcurrencyLimit, Assert.Single(updated.GetServices<OutboxSettings>()).ConcurrencyLimit);
        Assert.False(Assert.Single(updated.GetServices<JsonPayloadSerializerSettings>()).JsonOptions.PropertyNameCaseInsensitive);
    }

    [Theory]
    [InlineData("Inbox")]
    [InlineData("Outbox")]
    [InlineData("Json")]
    public void Configure_InvalidSettingsDoesNotPartiallyReplaceRegistrations(string invalid)
    {
        // Arrange
        var services = new ServiceCollection();
        services.ConfigureTychoEventProcessing(_ => { });
        ServiceDescriptor[] original = [.. services];

        // Act
        Exception? exception = Record.Exception(() => services.ConfigureTychoEventProcessing(options =>
        {
            if (invalid == "Inbox") options.Inbox.ConcurrencyLimit = 0;
            if (invalid == "Outbox") options.Outbox.ConcurrencyLimit = 0;
            if (invalid == "Json") options.PayloadSerializer.JsonOptions = null!;
        }));

        // Assert
        Assert.IsType<ArgumentException>(exception, exactMatch: false);
        Assert.Equal(original, [.. services]);
    }

    [Fact]
    public void Configure_RejectsNullArguments()
    {
        // Arrange
        var services = new ServiceCollection();
        IServiceCollection nullServices = null!;

        // Act
        void ConfigureWithNullCallback() => services.ConfigureTychoEventProcessing(null!);
        void ConfigureWithNullServices() => nullServices.ConfigureTychoEventProcessing(_ => { });

        // Assert
        Assert.Throws<ArgumentNullException>(ConfigureWithNullCallback);
        Assert.Throws<ArgumentNullException>(ConfigureWithNullServices);
    }

    [Fact]
    public void JsonSerializer_RequiresRegisteredSettingsAndUsesAnIndependentCopy()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IPayloadSerializer, JsonPayloadSerializer>();
        using ServiceProvider missingSettings = services.BuildServiceProvider();

        // Act
        Exception? missingSettingsException = Record.Exception(() => missingSettings.GetRequiredService<IPayloadSerializer>());
        services.ConfigureTychoEventProcessing(_ => { });
        using ServiceProvider provider = services.BuildServiceProvider();
        IPayloadSerializer serializer = provider.GetRequiredService<IPayloadSerializer>();
        JsonPayloadSerializerSettings settings = provider.GetRequiredService<JsonPayloadSerializerSettings>();
        string payload = serializer.Serialize(new TestEvent());
        settings.JsonOptions.PropertyNameCaseInsensitive = true;
        JsonPayloadSerializerSettings afterMutation = provider.GetRequiredService<JsonPayloadSerializerSettings>();
        settings.JsonOptions = null!;
        JsonPayloadSerializerSettings afterReplacement = provider.GetRequiredService<JsonPayloadSerializerSettings>();
        TestEvent restored = serializer.Deserialize<TestEvent>(payload);

        // Assert
        Assert.IsType<InvalidOperationException>(missingSettingsException);
        Assert.False(afterMutation.JsonOptions.PropertyNameCaseInsensitive);
        Assert.NotNull(afterReplacement.JsonOptions);
        Assert.NotNull(restored);
    }

    public sealed class TestEvent : IEvent { }
}
