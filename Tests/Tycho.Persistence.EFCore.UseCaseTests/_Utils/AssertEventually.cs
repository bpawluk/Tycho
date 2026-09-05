namespace Tycho.Persistence.EFCore.UseCaseTests._Utils;

public class AssertEventually
{
    public static async Task True(Func<Task<bool>> expression)
    {
        bool result = await expression();
        while (!result)
        {
            await Task.Delay(100);
            result = await expression();
        }
    }
}
