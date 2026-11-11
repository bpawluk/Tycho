using Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;
using Tycho.Persistence.EFCore.UseCaseTests._Utils;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore;

public sealed class OnlineStoreTests(DatabaseServers servers) : IAsyncLifetime
{
    private UseCaseDatabases? _databases;
    private readonly TestData _testData = new();
    private IOnlineStoreApp _sut = null!;

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    private async Task StartAppAsync()
    {
        _sut = new OnlineStoreApp().CreateAppBuilder().Build();
        await _sut.StartAsync(TestContext.Current.CancellationToken);
    }

    [Theory(Timeout = 360000)]
    [InlineData(DatabaseProvider.SQLite)]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    [InlineData(DatabaseProvider.MySql)]
    public async Task TychoUseCase_OnlineStoreApp_WorksCorrectly(DatabaseProvider provider)
    {
        _databases = new UseCaseDatabases(servers, provider);
        await _databases.InitializeAsync();
        using var binding = _databases.Bind();
        await StartAppAsync();

        await SetupProductCatalog();
        await AssertEventually.True(async () =>
        {
            var getProductsRequest = new GetProductsRequest();
            GetProductsRequest.Response response = await _sut.ExecuteAsync(getProductsRequest, TestContext.Current.CancellationToken);
            return _testData.InitialProducts.Match(response);
        });

        await BuyProducts();
        await AssertEventually.True(async () =>
        {
            var getProductsRequest = new GetProductsRequest();
            GetProductsRequest.Response response = await _sut.ExecuteAsync(getProductsRequest, TestContext.Current.CancellationToken);
            return _testData.GetProductsAfterPurchase().Match(response);
        });
        await AssertEventually.True(async () =>
        {
            var getBasketRequest = new GetBasketRequest(_testData.CustomerId);
            GetBasketRequest.Response response = await _sut.ExecuteAsync(getBasketRequest, TestContext.Current.CancellationToken);
            return _testData.GetBasket().Match(response);
        });

        await Checkout();
        await AssertEventually.True(async () =>
        {
            var getOrdersRequest = new GetOrdersRequest();
            GetOrdersRequest.Response response = await _sut.ExecuteAsync(getOrdersRequest, TestContext.Current.CancellationToken);
            return _testData.GetOrders().Match(response);
        });
    }

    private async Task SetupProductCatalog()
    {
        foreach (TestData.Product product in _testData.InitialProducts)
        {
            var createProductRequest = new CreateProductRequest(product.Name, product.Price);
            CreateProductRequest.Response response = await _sut.ExecuteAsync(createProductRequest, TestContext.Current.CancellationToken);
            product.Id = response.CreatedProductId;

            var stockProductRequest = new StockItemRequest(product.Id.Value, product.Quantity);
            await _sut.ExecuteAsync(stockProductRequest, TestContext.Current.CancellationToken);
        }
    }

    private async Task BuyProducts()
    {
        foreach (TestData.BasketItem item in _testData.GetBasket())
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
        try
        {
            if (_sut is not null)
                await _sut.StopAsync();
        }
        finally
        {
            try
            {
                _sut?.Dispose();
            }
            finally
            {
                if (_databases is not null)
                    await _databases.DisposeAsync();
            }
        }
    }
}
