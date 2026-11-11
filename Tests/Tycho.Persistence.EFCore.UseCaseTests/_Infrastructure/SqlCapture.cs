using System.Collections.Concurrent;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;

internal sealed class SqlCapture : DbCommandInterceptor
{
    private readonly ConcurrentQueue<string> _commands = new();
    private readonly ConcurrentQueue<string> _failures = new();
    private readonly ConcurrentDictionary<string, string> _plans = new();

    internal string[] Commands => _commands.ToArray();
    internal string[] Failures => _failures.ToArray();
    internal string[] Plans => _plans.Values.ToArray();

    public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
        => RecordFailure(command, eventData);

    public override Task CommandFailedAsync(DbCommand command, CommandErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        RecordFailure(command, eventData);
        return Task.CompletedTask;
    }

    private void RecordFailure(DbCommand command, CommandErrorEventData eventData)
    {
        _failures.Enqueue($"{eventData.Exception.GetType().Name}: {eventData.Exception.Message}{Environment.NewLine}{command.CommandText}");
        while (_failures.Count > 16)
            _failures.TryDequeue(out _);
    }

    private void Record(DbCommand command)
    {
        // Record SQL and parameter metadata, never connection strings or credentials.
        string parameters = string.Join(", ", command.Parameters.Cast<DbParameter>()
            .Select(parameter => $"{parameter.ParameterName} ({parameter.DbType})"));
        _commands.Enqueue($"{command.CommandText}{Environment.NewLine}Parameters: {parameters}");
        // Retain the useful tail without flooding failure output with worker polls.
        while (_commands.Count > 128)
            _commands.TryDequeue(out _);
    }

    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        Record(command);
        return result;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
    {
        Record(command);
        return ValueTask.FromResult(result);
    }

    public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
    {
        Record(command);
        return result;
    }

    public override async ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        Record(command);
        if (DatabaseServers.MySqlDeadlockDiagnosticsEnabled &&
            command.CommandText.StartsWith("UPDATE", StringComparison.Ordinal) &&
            command.CommandText.Contains("`EntryId` =", StringComparison.Ordinal) &&
            _plans.TryAdd(command.CommandText, "Query plan capture pending"))
        {
            try
            {
                // EXPLAIN does not execute the update. Use the same connection,
                // transaction and typed parameters to inspect its actual access path.
                await using DbCommand explain = command.Connection!.CreateCommand();
                explain.Transaction = command.Transaction;
                explain.CommandTimeout = command.CommandTimeout;
                explain.CommandText = "EXPLAIN FORMAT=JSON " + command.CommandText;
                foreach (DbParameter parameter in command.Parameters)
                {
                    DbParameter copy = explain.CreateParameter();
                    copy.ParameterName = parameter.ParameterName;
                    copy.DbType = parameter.DbType;
                    copy.Size = parameter.Size;
                    copy.Precision = parameter.Precision;
                    copy.Scale = parameter.Scale;
                    copy.Value = parameter.Value;
                    explain.Parameters.Add(copy);
                }
                object? plan = await explain.ExecuteScalarAsync(cancellationToken);
                _plans[command.CommandText] = $"{command.CommandText}{Environment.NewLine}{plan}";
            }
            catch (Exception exception)
            {
                _plans[command.CommandText] = $"Could not capture query plan: {exception.Message}";
            }
        }
        return result;
    }

    public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
    {
        Record(command);
        return result;
    }

    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
    {
        Record(command);
        return ValueTask.FromResult(result);
    }
}
