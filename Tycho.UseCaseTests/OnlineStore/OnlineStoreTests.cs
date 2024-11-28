using Tycho.Structure;
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
    private IApp _sut = null!;

    public async Task InitializeAsync()
    {
       _sut = await new OnlineStoreApp().Run();
    }

    [Fact(Timeout = 2500)]
    public async Task TychoUseCase_OnlineStoreApp_WorksCorrectly()
    {
        await SetupProductCatalog();
        await AssertEventually.True(async () => 
        {
            var getProductsRequest = new GetProductsRequest();
            var response = await _sut.Execute<GetProductsRequest, GetProductsRequest.Response>(getProductsRequest);
            return _testData.InitialProducts.Match(response);
        });

        await BuyProducts();
        await AssertEventually.True(async () =>
        {
            var getProductsRequest = new GetProductsRequest();
            var response = await _sut.Execute<GetProductsRequest, GetProductsRequest.Response>(getProductsRequest);
            return _testData.GetProductsAfterPurchase().Match(response);
        });
        await AssertEventually.True(async () =>
        {
            var getBasketRequest = new GetBasketRequest(_testData.CustomerId);
            var response = await _sut.Execute<GetBasketRequest, GetBasketRequest.Response>(getBasketRequest);
            return _testData.GetBasket().Match(response);
        });

        await Checkout();
        await AssertEventually.True(async () =>
        {
            var getOrdersRequest = new GetOrdersRequest();
            var response = await _sut.Execute<GetOrdersRequest, GetOrdersRequest.Response>(getOrdersRequest);
            return _testData.GetOrders().Match(response);
        });
    }

    private async Task SetupProductCatalog()
    {
        foreach (var product in _testData.InitialProducts)
        {
            var createProductRequest = new CreateProductRequest(product.Name, product.Price);
            var response = await _sut.Execute<CreateProductRequest, CreateProductRequest.Response>(createProductRequest);
            product.Id = response.CreatedProductId;

            var stockProductRequest = new StockItemRequest(product.Id.Value, product.Quantity);
            await _sut.Execute(stockProductRequest);
        }
    }

    private async Task BuyProducts()
    {
        foreach (var item in _testData.GetBasket())
        {
            var butProductRequest = new BuyProductRequest(_testData.CustomerId, item.ProductId, item.Quantity);
            await _sut.Execute(butProductRequest);
        }
    }

    private async Task Checkout()
    {
        var checkoutRequest = new CheckoutRequest(_testData.CustomerId);
        await _sut.Execute(checkoutRequest);
    }

    public async Task DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}