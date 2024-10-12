using System.Collections.Concurrent;

namespace Tycho.IntegrationTests._Utils;

internal class CompoundResult<TSubResult>(IReadOnlyCollection<TSubResult> expectedSubResults)
{
    private readonly IReadOnlyCollection<TSubResult> _expectedSubResults = expectedSubResults;

    private readonly ConcurrentBag<TSubResult> _actualSubResults = [];

    public bool IsComplete => _expectedSubResults.Except(_actualSubResults).Any() == false;

    public void AddSubResult(TSubResult subResult)
    {
        if (_expectedSubResults.Contains(subResult))
        {
            _actualSubResults.Add(subResult);
        }
    }
}
