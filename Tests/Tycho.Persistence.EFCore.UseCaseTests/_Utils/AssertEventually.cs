extern alias TychoInternals;

// Internal test access also exposes Tycho's compatibility attribute.
using CallerArgumentExpressionAttribute = TychoInternals::System.Runtime.CompilerServices.CallerArgumentExpressionAttribute;

namespace Tycho.Persistence.EFCore.UseCaseTests._Utils;

public class AssertEventually
{
    public static async Task True(
        Func<Task<bool>> expression,
        [CallerArgumentExpression(nameof(expression))] string? assertion = null)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(30));
        try
        {
            while (!await expression())
            {
                await Task.Delay(100, timeout.Token);
            }
        }
        catch (OperationCanceledException) when (timeout.IsCancellationRequested && !TestContext.Current.CancellationToken.IsCancellationRequested)
        {
            Assert.Fail($"Condition did not become true within 30 seconds:{Environment.NewLine}{assertion}");
        }
    }
}
