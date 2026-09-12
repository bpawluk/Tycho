using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Domain;

internal class BasketProvider(BasketDbContext dbContext)
{
    public async Task<Basket> GetBasket(int customerId, CancellationToken cancellationToken)
    {
        Basket? customerBasket = await dbContext.Baskets.SingleOrDefaultAsync(
            basket => basket.CustomerId == customerId && !basket.CheckedOut,
            cancellationToken);

        if (customerBasket is null)
        {
            customerBasket = new Basket(customerId);
            dbContext.Baskets.Add(customerBasket);
        }

        return customerBasket;
    }
}
