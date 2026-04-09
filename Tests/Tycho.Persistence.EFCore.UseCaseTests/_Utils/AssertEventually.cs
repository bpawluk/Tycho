namespace Tycho.UseCaseTests._Utils;

public class AssertEventually
{
    public static async Task True(Func<Task<bool>> expression)
    {
        var result = await expression();
        while (!result)
        {
            await Task.Delay(100);
            result = await expression();
        }
    }
}