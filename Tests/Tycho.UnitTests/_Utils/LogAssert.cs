using Microsoft.Extensions.Logging;
using Moq;

namespace Tycho.UnitTests._Utils;

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

    public static async Task WaitForLogAsync<T>(Mock<ILogger<T>> logger, int eventId, CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(5));

        while (!logger.Invocations.Any(invocation => invocation.Method.Name == nameof(ILogger.Log) && invocation.Arguments[1] is EventId id && id.Id == eventId))
        {
            await Task.Delay(10, timeout.Token);
        }
    }

    private static bool HasProperties(object state, (string Name, object? Value)[] properties)
    {
        return state is IEnumerable<KeyValuePair<string, object?>> values &&
               properties.All(property => values.Any(item => item.Key == property.Name && Equals(item.Value, property.Value)));
    }
}
