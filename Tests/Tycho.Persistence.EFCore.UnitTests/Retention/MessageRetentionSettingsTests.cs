using Tycho.Persistence.EFCore.Retention;

namespace Tycho.Persistence.EFCore.UnitTests.Retention;

public sealed class MessageRetentionSettingsTests
{
    [Theory]
    [InlineData(null, null, false)]
    [InlineData(1, null, true)]
    [InlineData(null, 1, true)]
    [InlineData(1, 2, true)]
    public void IsEnabled_WithRetentionSettings_ReturnsExpectedValue(
        int? payloadDays,
        int? cleanupDays,
        bool expected)
    {
        // Arrange
        var options = new MessageRetentionSettings
        {
            PayloadRetention = payloadDays is int payload ? TimeSpan.FromDays(payload) : null,
            FullCleanupRetention = cleanupDays is int cleanup ? TimeSpan.FromDays(cleanup) : null
        };

        // Act
        bool enabled = options.IsEnabled;

        // Assert
        Assert.Equal(expected, enabled);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(1, null)]
    [InlineData(null, 1)]
    [InlineData(1, 2)]
    public void Validate_WithValidRetentionSettings_DoesNotThrow(int? payloadDays, int? cleanupDays)
    {
        // Arrange
        var options = new MessageRetentionSettings
        {
            PayloadRetention = payloadDays is int payload ? TimeSpan.FromDays(payload) : null,
            FullCleanupRetention = cleanupDays is int cleanup ? TimeSpan.FromDays(cleanup) : null
        };

        // Act
        Exception? exception = Record.Exception(options.Validate);

        // Assert
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositivePayloadRetention_Throws(int minutes)
    {
        // Arrange
        var options = new MessageRetentionSettings { PayloadRetention = TimeSpan.FromMinutes(minutes) };

        // Act
        Action act = options.Validate;

        // Assert
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(act);

        Assert.Equal(nameof(options.PayloadRetention), exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositiveFullCleanupRetention_Throws(int minutes)
    {
        // Arrange
        var options = new MessageRetentionSettings { FullCleanupRetention = TimeSpan.FromMinutes(minutes) };

        // Act
        Action act = options.Validate;

        // Assert
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(act);

        Assert.Equal(nameof(options.FullCleanupRetention), exception.ParamName);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    public void Validate_WithPayloadRetentionNotShorterThanFullCleanup_Throws(int payloadDays, int cleanupDays)
    {
        // Arrange
        var options = new MessageRetentionSettings
        {
            PayloadRetention = TimeSpan.FromDays(payloadDays),
            FullCleanupRetention = TimeSpan.FromDays(cleanupDays)
        };

        // Act
        Action act = options.Validate;

        // Assert
        Assert.Throws<ArgumentException>(act);
    }
}
