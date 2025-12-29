using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Contract.Incoming;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.Incoming;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Contract;

namespace Tycho.UseCaseTests.OnlineStore;

internal class TestData
{
    public int CustomerId { get; } = 1;

    public Catalog InitialProducts { get; } =
    [
        new("A4 Printing Paper (500 Sheets)", 24.99M, 100),
        new("Legal Size Paper (500 Sheets)", 19.99M, 75),
        new("Heavyweight Cardstock (100 Sheets)", 29.99M, 50)
    ];

    public Basket GetBasket() =>
    [
        new(InitialProducts[0].Id!.Value, 50, InitialProducts[0].Price, "Confirmed"),
        new(InitialProducts[2].Id!.Value, 25, InitialProducts[2].Price, "Confirmed")
    ];

    public Catalog GetProductsAfterPurchase()
    {
        var basket = GetBasket();
        var productsAfterPurchase = new Catalog();
        foreach (var product in InitialProducts)
        {
            var basketItem = basket.FirstOrDefault(item => item.ProductId == product.Id);
            var updatedProduct = product with 
            { 
                Quantity = product.Quantity - basketItem?.Quantity ?? 0 
            };
            productsAfterPurchase.Add(product);
        }
        return productsAfterPurchase;
    }

    public Orders GetOrders() =>
    [
        new(CustomerId, GetBasket().Sum(item => item.Quantity * item.Price))
    ];

    public class Catalog : List<Product> 
    { 
        public bool Match(GetProductsRequest.Response response)
        {
            return Count == response.Products.Count &&
                   this.All(product => response.Products.Any(product.Matches));
        }
    }

    public record Product(string Name, decimal Price, uint Quantity)
    {
        public int? Id { get; set; }

        public bool Matches(GetProductsRequest.Product fetchedProduct)
        {
            return Id == fetchedProduct.Id &&
                   Name == fetchedProduct.Name &&
                   Price == fetchedProduct.Price &&
                   Quantity == fetchedProduct.Availability;
        }
    }

    public class Basket : List<BasketItem> 
    {
        public bool Match(GetBasketRequest.Response response)
        {
            return Count == response.BasketItems.Count &&
                   this.All(item => response.BasketItems.Any(item.Matches));
        }
    }

    public record BasketItem(int ProductId, uint Quantity, decimal Price, string Status)
    {
        public bool Matches(GetBasketRequest.BasketItem fetchedBasketItem)
        {
            return ProductId == fetchedBasketItem.ProductId &&
                   Quantity == fetchedBasketItem.Quantity &&
                   Price == fetchedBasketItem.Price &&
                   Status == fetchedBasketItem.Status;
        }
    }

    public class Orders : List<Order>
    {
        public bool Match(GetOrdersRequest.Response response)
        {
            return Count == response.Orders.Count &&
                   this.All(item => response.Orders.Any(item.Matches));
        }
    }

    public record Order(int CustomerId, decimal Total)
    {
        public bool Matches(GetOrdersRequest.Order fetchedOrder)
        {
            return CustomerId == fetchedOrder.CustomerId &&
                   Total == fetchedOrder.Total;
        }
    }
}