using Tycho.Persistence.EFCore.UseCaseTests._Utils;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore;

public sealed class OnlineStoreTests : IAsyncLifetime
{
    private readonly TestData _testData = new();
    private IOnlineStoreApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = await new OnlineStoreApp().RunAsync();
    }

    [Fact(Timeout = 10000)]
    public async Task TychoUseCase_OnlineStoreApp_WorksCorrectly()
    {
        await SetupProductCatalog();
        await AssertEventually.True(async () =>
        {
            var getProductsRequest = new GetProductsRequest();
            var response = await _sut.ExecuteAsync(getProductsRequest, TestContext.Current.CancellationToken);
            return _testData.InitialProducts.Match(response);
        });

        await BuyProducts();
        await AssertEventually.True(async () =>
        {
            var getProductsRequest = new GetProductsRequest();
            var response = await _sut.ExecuteAsync(getProductsRequest, TestContext.Current.CancellationToken);
            return _testData.GetProductsAfterPurchase().Match(response);
        });
        await AssertEventually.True(async () =>
        {
            var getBasketRequest = new GetBasketRequest(_testData.CustomerId);
            var response = await _sut.ExecuteAsync(getBasketRequest, TestContext.Current.CancellationToken);
            return _testData.GetBasket().Match(response);
        });

        await Checkout();
        await AssertEventually.True(async () =>
        {
            var getOrdersRequest = new GetOrdersRequest();
            var response = await _sut.ExecuteAsync(getOrdersRequest, TestContext.Current.CancellationToken);
            return _testData.GetOrders().Match(response);
        });
    }

    private async Task SetupProductCatalog()
    {
        foreach (var product in _testData.InitialProducts)
        {
            var createProductRequest = new CreateProductRequest(product.Name, product.Price);
            var response = await _sut.ExecuteAsync(createProductRequest, TestContext.Current.CancellationToken);
            product.Id = response.CreatedProductId;

            var stockProductRequest = new StockItemRequest(product.Id.Value, product.Quantity);
            await _sut.ExecuteAsync(stockProductRequest, TestContext.Current.CancellationToken);
        }
    }

    private async Task BuyProducts()
    {
        foreach (var item in _testData.GetBasket())
        {
            var butProductRequest = new BuyProductRequest(_testData.CustomerId, item.ProductId, item.Quantity);
            await _sut.ExecuteAsync(butProductRequest, TestContext.Current.CancellationToken);
        }
    }

    private async Task Checkout()
    {
        var checkoutRequest = new CheckoutRequest(_testData.CustomerId);
        await _sut.ExecuteAsync(checkoutRequest, TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}
