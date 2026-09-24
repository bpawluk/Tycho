using Tycho.Persistence.EFCore.Retention;

namespace Tycho.Persistence.EFCore.UnitTests.Retention;

public sealed class PersistenceRetentionOptionsTests
{
    [Fact]
    public void Defaults_ProvideValidRetentionSettings()
    {
        // Arrange
        var options = new PersistenceRetentionOptions();

        // Act
        Exception? exception = Record.Exception(options.Validate);

        // Assert
        Assert.Equal(TimeSpan.FromHours(1), options.CleanupInterval);
        Assert.Equal(TimeSpan.FromDays(7), options.Inbox.PayloadRetention);
        Assert.Null(options.Inbox.FullCleanupRetention);
        Assert.Null(options.Outbox.PayloadRetention);
        Assert.Equal(TimeSpan.FromDays(7), options.Outbox.FullCleanupRetention);
        Assert.True(options.IsEnabled);
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
        var options = new PersistenceRetentionOptions();
        options.Inbox.PayloadRetention = enableInbox ? TimeSpan.FromDays(1) : null;
        options.Outbox.FullCleanupRetention = enableOutbox ? TimeSpan.FromDays(1) : null;

        // Act
        bool enabled = options.IsEnabled;

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
        var options = new PersistenceRetentionOptions
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
        var options = new PersistenceRetentionOptions
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
        var options = new PersistenceRetentionOptions();
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
        var options = new PersistenceRetentionOptions();
        options.Outbox.FullCleanupRetention = TimeSpan.Zero;

        // Act
        Action act = options.Validate;

        // Assert
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(act);

        Assert.Equal(nameof(options.Outbox.FullCleanupRetention), exception.ParamName);
    }
}
