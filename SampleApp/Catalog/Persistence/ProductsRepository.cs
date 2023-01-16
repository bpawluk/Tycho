using SampleApp.Catalog.Model;
using System.Collections.Generic;
using System.Linq;

namespace SampleApp.Catalog.Persistence;

internal class ProductsRepository : IProductsRepository
{
    private readonly List<Product> _productsData = new()
    {
        new Product("product-01", "Mug", 10, 100),
        new Product("product-02", "T-shirt", 25, 100),
        new Product("product-03", "Wristwatch", 50, 100)
    };

    public IEnumerable<Product> GetProducts() => _productsData;

    public Product? FindProduct(string nameQuery)
    {
        return _productsData.FirstOrDefault(p => p.Name.ToLower().Contains(nameQuery.ToLower()));
    }

    public void UpdatePrice(string productId, decimal newPrice)
    {
        var product = _productsData.FirstOrDefault(p => p.Id == productId);
        if (product is not null)
        {
            product.Price = newPrice;
        }
    }

    public void UpdateStockLevel(string productId, int newLevel)
    {
        var product = _productsData.FirstOrDefault(p => p.Id == productId);
        if (product is not null)
        {
            product.StockLevel = newLevel;
        }
    }
}
