using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;

internal sealed class UseCaseLogCapture : ILoggerProvider
{
    private readonly ConcurrentQueue<string> _messages = new();

    internal string[] Messages => _messages.ToArray();

    public ILogger CreateLogger(string categoryName) => new CaptureLogger(categoryName, _messages);

    public void Dispose() { }

    private sealed class CaptureLogger(string category, ConcurrentQueue<string> messages) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Warning;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;
            messages.Enqueue($"{logLevel} {category}: {formatter(state, exception)}{Environment.NewLine}{exception}");
            while (messages.Count > 32)
                messages.TryDequeue(out _);
        }
    }
}
