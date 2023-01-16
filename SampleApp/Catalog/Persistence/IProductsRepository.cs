using SampleApp.Catalog.Model;
using System.Collections.Generic;

namespace SampleApp.Catalog.Persistence;

internal interface IProductsRepository
{
    IEnumerable<Product> GetProducts();
    Product? FindProduct(string nameQuery);
    void UpdatePrice(string productId, decimal newPrice);
    void UpdateStockLevel(string productId, int newLevel);
}
