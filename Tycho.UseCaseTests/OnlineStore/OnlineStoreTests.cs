using Tycho.Structure;
using Tycho.UseCaseTests.OnlineStore.SUT;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract;

namespace Tycho.UseCaseTests.OnlineStore;

public class OnlineStoreTests : IAsyncLifetime
{
    private IApp _sut = null!;

    public async Task InitializeAsync()
    {
       _sut = await new OnlineStoreApp().Run();
    }

    // Times X:
    // - CreateProductRequest
    // - StockItemRequest
    // Confirm with GetProductsRequest
    // Times Y:
    // - AddBasketItemRequest
    // Confirm with GetProductsRequest
    // Checkout
    // Get Orders
    // Confirm with GetProductsRequest

    [Fact(Timeout = 2500)]
    public async Task TychoUseCase_OnlineStoreApp_WorksCorrectly()
    {
        await DefineStoreProducts();
        await Task.Delay(1500);
    }

    private async Task DefineStoreProducts()
    {
        var createFirstProductRequest = new CreateProductRequest("A4 Printing Paper (500 Sheets)", 24.99M);
        var firstResponse = await _sut.Execute<CreateProductRequest, CreateProductRequest.Response>(createFirstProductRequest);

        var stockFirstProductRequest = new StockItemRequest(firstResponse.CreatedProductId, 100);
        await _sut.Execute(stockFirstProductRequest);

        var createSecondProductRequest = new CreateProductRequest("Legal Size Paper (500 Sheets)", 19.99M);
        var secondResponse = await _sut.Execute<CreateProductRequest, CreateProductRequest.Response>(createSecondProductRequest);

        var stockSecondProductRequest = new StockItemRequest(secondResponse.CreatedProductId, 75);
        await _sut.Execute(stockSecondProductRequest);

        var createThirdProductRequest = new CreateProductRequest("Heavyweight Cardstock (100 Sheets)", 29.99M);
        var thirdResponse = await _sut.Execute<CreateProductRequest, CreateProductRequest.Response>(createThirdProductRequest);

        var stockThirdProductRequest = new StockItemRequest(thirdResponse.CreatedProductId, 50);
        await _sut.Execute(stockThirdProductRequest);
    }

    private async Task AddBasketItems()
    {

    }

    public Task DisposeAsync()
    {
        _sut?.Dispose();
        return Task.CompletedTask;
    }
}