namespace SampleApp.Catalog.Model;

public class Product
{
    public string Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int StockLevel { get; set; }

    public Product(string id, string name, decimal price, int stockLevel)
    {
        Id = id; 
        Name = name; 
        Price = price;
        StockLevel = stockLevel;
    }

    public override string ToString()
    {
        return $"> {Id} - {Name,-10} - {Price:C} - {StockLevel,3}";
    }
}
