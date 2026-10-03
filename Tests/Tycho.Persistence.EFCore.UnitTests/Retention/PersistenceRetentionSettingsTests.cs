using Microsoft.Extensions.DependencyInjection;
using Tycho.Persistence.EFCore.Retention;

namespace Tycho.Persistence.EFCore.UnitTests.Retention;

public sealed class PersistenceRetentionSettingsTests
{
    [Fact]
    public void Defaults_ProvideValidRetentionSettings()
    {
        // Arrange
        var options = new PersistenceRetentionSettings();

        // Act
        Exception? exception = Record.Exception(options.Validate);

        // Assert
        Assert.Equal(TimeSpan.FromMinutes(5), options.InitialDelay);
        Assert.Equal(TimeSpan.FromHours(1), options.CleanupInterval);
        Assert.Equal(TimeSpan.FromDays(7), options.Inbox.PayloadRetention);
        Assert.Null(options.Inbox.FullCleanupRetention);
        Assert.Null(options.Outbox.PayloadRetention);
        Assert.Equal(TimeSpan.FromDays(7), options.Outbox.FullCleanupRetention);
        Assert.True(options.IsRetentionEnabled);
        Assert.Null(exception);
    }

    [Fact]
    public void Registration_IsolatesNestedRetentionSettingsPerResolution()
    {
        // Arrange
        var services = new ServiceCollection();
        PersistenceRetentionSettings? original = null;
        services.AddTychoPersistenceRetention(options => original = options);
        using ServiceProvider provider = services.BuildServiceProvider();
        PersistenceRetentionSettings registered = provider.GetRequiredService<PersistenceRetentionSettings>();

        // Act
        original!.InitialDelay = TimeSpan.Zero;
        original.Inbox.PayloadRetention = TimeSpan.FromDays(1);
        TimeSpan registeredInitialDelay = registered.InitialDelay;
        TimeSpan? registeredPayloadRetention = registered.Inbox.PayloadRetention;
        registered.InitialDelay = TimeSpan.Zero;
        registered.CleanupInterval = TimeSpan.Zero;
        registered.Inbox.PayloadRetention = TimeSpan.Zero;
        registered.Outbox.FullCleanupRetention = TimeSpan.Zero;
        PersistenceRetentionSettings next = provider.GetRequiredService<PersistenceRetentionSettings>();

        // Assert
        Assert.Equal(TimeSpan.FromMinutes(5), registeredInitialDelay);
        Assert.Equal(TimeSpan.FromDays(7), registeredPayloadRetention);
        Assert.NotSame(registered, next);
        Assert.NotSame(registered.Inbox, next.Inbox);
        Assert.NotSame(registered.Outbox, next.Outbox);
        Assert.Equal(TimeSpan.FromMinutes(5), next.InitialDelay);
        Assert.Equal(TimeSpan.FromHours(1), next.CleanupInterval);
        Assert.Equal(TimeSpan.FromDays(7), next.Inbox.PayloadRetention);
        Assert.Equal(TimeSpan.FromDays(7), next.Outbox.FullCleanupRetention);
    }

    [Theory]
    [InlineData(-1L)]
    [InlineData(4294967295L)]
    public void Validate_RejectsUnsupportedInitialDelay(long milliseconds)
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTychoPersistenceRetention();
        ServiceDescriptor[] original = [.. services];

        // Act
        void Act() => services.AddTychoPersistenceRetention(options => options.InitialDelay = TimeSpan.FromMilliseconds(milliseconds));

        // Assert
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(Act);
        Assert.Equal("InitialDelay", exception.ParamName);
        Assert.Equal(original, [.. services]);
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(4294967294L)]
    public void Validate_AcceptsSupportedInitialDelay(long milliseconds)
    {
        // Arrange
        var options = new PersistenceRetentionSettings { InitialDelay = TimeSpan.FromMilliseconds(milliseconds) };

        // Act
        Exception? exception = Record.Exception(options.Validate);

        // Assert
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    [InlineData(true, true, true)]
    public void IsEnabled_WithInboxAndOutboxSettings_ReturnsExpectedValue(
        bool enableInbox,
        bool enableOutbox,
        bool expected)
    {
        // Arrange
        var options = new PersistenceRetentionSettings();
        options.Inbox.PayloadRetention = enableInbox ? TimeSpan.FromDays(1) : null;
        options.Outbox.FullCleanupRetention = enableOutbox ? TimeSpan.FromDays(1) : null;

        // Act
        bool enabled = options.IsRetentionEnabled;

        // Assert
        Assert.Equal(expected, enabled);
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(-1L)]
    [InlineData(4294967295L)]
    public void Validate_WithUnsupportedCleanupInterval_Throws(long milliseconds)
    {
        // Arrange
        var options = new PersistenceRetentionSettings
        {
            CleanupInterval = TimeSpan.FromMilliseconds(milliseconds)
        };

        // Act
        Action act = options.Validate;

        // Assert
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(act);

        Assert.Equal(nameof(options.CleanupInterval), exception.ParamName);
    }

    [Theory]
    [InlineData(1L)]
    [InlineData(4294967294L)]
    public void Validate_WithSupportedCleanupInterval_DoesNotThrow(long milliseconds)
    {
        // Arrange
        var options = new PersistenceRetentionSettings
        {
            CleanupInterval = TimeSpan.FromMilliseconds(milliseconds)
        };

        // Act
        Exception? exception = Record.Exception(options.Validate);

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void Validate_WithInvalidInboxSettings_Throws()
    {
        // Arrange
        var options = new PersistenceRetentionSettings();
        options.Inbox.PayloadRetention = TimeSpan.Zero;

        // Act
        Action act = options.Validate;

        // Assert
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(act);

        Assert.Equal(nameof(options.Inbox.PayloadRetention), exception.ParamName);
    }

    [Fact]
    public void Validate_WithInvalidOutboxSettings_Throws()
    {
        // Arrange
        var options = new PersistenceRetentionSettings();
        options.Outbox.FullCleanupRetention = TimeSpan.Zero;

        // Act
        Action act = options.Validate;

        // Assert
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(act);

        Assert.Equal(nameof(options.Outbox.FullCleanupRetention), exception.ParamName);
    }
}
