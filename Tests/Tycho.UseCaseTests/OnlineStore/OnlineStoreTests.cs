using Tycho.UseCaseTests._Utils;
using Tycho.UseCaseTests.OnlineStore.SUT;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Incoming;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;

namespace Tycho.UseCaseTests.OnlineStore;

public class OnlineStoreTests : IAsyncLifetime
{
    private readonly TestData _testData = new();
    private IOnlineStoreApp _sut = null!;

    public async Task InitializeAsync()
    {
       _sut = await new OnlineStoreApp().RunAsync();
    }

    [Fact(Timeout = 2500)]
    public async Task TychoUseCase_OnlineStoreApp_WorksCorrectly()
    {
        await SetupProductCatalog();
        await AssertEventually.True(async () => 
        {
            var getProductsRequest = new GetProductsRequest();
            var response = await _sut.ExecuteAsync(getProductsRequest);
            return _testData.InitialProducts.Match(response);
        });

        await BuyProducts();
        await AssertEventually.True(async () =>
        {
            var getProductsRequest = new GetProductsRequest();
            var response = await _sut.ExecuteAsync(getProductsRequest);
            return _testData.GetProductsAfterPurchase().Match(response);
        });
        await AssertEventually.True(async () =>
        {
            var getBasketRequest = new GetBasketRequest(_testData.CustomerId);
            var response = await _sut.ExecuteAsync(getBasketRequest);
            return _testData.GetBasket().Match(response);
        });

        await Checkout();
        await AssertEventually.True(async () =>
        {
            var getOrdersRequest = new GetOrdersRequest();
            var response = await _sut.ExecuteAsync(getOrdersRequest);
            return _testData.GetOrders().Match(response);
        });
    }

    private async Task SetupProductCatalog()
    {
        foreach (var product in _testData.InitialProducts)
        {
            var createProductRequest = new CreateProductRequest(product.Name, product.Price);
            var response = await _sut.ExecuteAsync(createProductRequest);
            product.Id = response.CreatedProductId;

            var stockProductRequest = new StockItemRequest(product.Id.Value, product.Quantity);
            await _sut.ExecuteAsync(stockProductRequest);
        }
    }

    private async Task BuyProducts()
    {
        foreach (var item in _testData.GetBasket())
        {
            var butProductRequest = new BuyProductRequest(_testData.CustomerId, item.ProductId, item.Quantity);
            await _sut.ExecuteAsync(butProductRequest);
        }
    }

    private async Task Checkout()
    {
        var checkoutRequest = new CheckoutRequest(_testData.CustomerId);
        await _sut.ExecuteAsync(checkoutRequest);
    }

    public async Task DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}