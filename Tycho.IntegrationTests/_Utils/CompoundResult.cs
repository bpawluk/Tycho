using System.Collections.Concurrent;

namespace Tycho.IntegrationTests._Utils;

internal class CompoundResult<TSubResult>(IReadOnlyCollection<TSubResult> expectedSubResults)
{
    private readonly ConcurrentBag<TSubResult> _actualSubResults = [];
    private readonly IReadOnlyCollection<TSubResult> _expectedSubResults = expectedSubResults;

    public bool IsComplete => !_expectedSubResults.Except(_actualSubResults).Any();

    public void AddSubResult(TSubResult subResult)
    {
        if (_expectedSubResults.Contains(subResult))
        {
            _actualSubResults.Add(subResult);
        }
    }
}