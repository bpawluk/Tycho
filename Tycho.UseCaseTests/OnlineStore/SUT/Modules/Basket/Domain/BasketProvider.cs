using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Domain;

internal class BasketProvider(IUnitOfWork unitOfWork)
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Basket> GetBasket(int customerId, CancellationToken cancellationToken)
    {
        var baskets = _unitOfWork.Set<Basket>();

        var customerBasket = await baskets.SingleOrDefaultAsync(
            basket => basket.CustomerId == customerId &&
                     !basket.CheckedOut,
            cancellationToken);

        if (customerBasket is null)
        {
            customerBasket = new Basket(customerId);
            baskets.Add(customerBasket);
        }

        return customerBasket;
    }
}