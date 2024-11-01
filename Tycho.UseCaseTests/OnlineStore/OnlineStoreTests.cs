using Tycho.Structure;
using Tycho.UseCaseTests._Utils;
using Tycho.UseCaseTests.OnlineStore.SUT;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract;

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

        await AddProductsToBasket();
        // Confirm with GetProductsRequest

        await Checkout();
        // Confirm with Get Orders
        // Confirm with GetProductsRequest
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

    private async Task AddProductsToBasket()
    {

    }

    private async Task Checkout()
    {

    }

    public Task DisposeAsync()
    {
        _sut?.Dispose();
        return Task.CompletedTask;
    }
}