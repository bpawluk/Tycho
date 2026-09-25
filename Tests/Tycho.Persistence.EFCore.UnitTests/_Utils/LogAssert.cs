using Microsoft.Extensions.Logging;
using Moq;

namespace Tycho.Persistence.EFCore.UnitTests._Utils;

internal static class LogAssert
{
    public static void Logged<T>(
        Mock<ILogger<T>> logger,
        LogLevel level,
        int eventId,
        string eventName,
        Exception? exception = null,
        params (string Name, object? Value)[] properties)
    {
        logger.Verify(item => item.Log(
            level,
            It.Is<EventId>(id => id.Id == eventId && id.Name == eventName),
            It.Is<It.IsAnyType>((state, _) => HasProperties(state, properties)),
            It.Is<Exception?>(loggedException => ReferenceEquals(loggedException, exception)),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    private static bool HasProperties(object state, (string Name, object? Value)[] properties)
    {
        return state is IEnumerable<KeyValuePair<string, object?>> values &&
               properties.All(property => values.Any(item => item.Key == property.Name && Equals(item.Value, property.Value)));
    }
}
