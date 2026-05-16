using Microsoft.Extensions.Logging;

namespace Tycho.IntegrationTests.ConfiguringLogging;

internal class TestLogger : ILogger
{
    public List<string> Logs { get; } = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return new Scope();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        Logs.Add(formatter(state, exception));
    }
}

internal class Scope : IDisposable
{
    public void Dispose() { }
}

internal class TestLoggerProvider : ILoggerProvider
{
    private static readonly TestLogger s_logger = new();

    public ILogger CreateLogger(string categoryName)
    {
        return s_logger;
    }

    public void Dispose() { }
}
