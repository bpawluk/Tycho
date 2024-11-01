using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;

namespace Tycho.UseCaseTests.OnlineStore;

internal class TestData
{
    public Products InitialProducts { get; } =
    [
        new("A4 Printing Paper (500 Sheets)", 24.99M, 100),
        new("Legal Size Paper (500 Sheets)", 19.99M, 75),
        new("Heavyweight Cardstock (100 Sheets)", 29.99M, 50)
    ];

    public class Products : List<Product> 
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

        public bool Matches(GetProductsRequest.Response.Product fetchedProduct)
        {
            return Id == fetchedProduct.Id &&
                   Name == fetchedProduct.Name &&
                   Price == fetchedProduct.Price &&
                   Quantity == fetchedProduct.Availability;
        }
    }
}