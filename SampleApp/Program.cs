using SampleApp.Catalog;
using SampleApp.Catalog.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tycho;

namespace SampleApp.App;

internal class Program
{
    static async Task Main(string[] args)
    {
        var app = await new AppModule().Build();

        Console.WriteLine("Start with the following store catalog:");
        await PrintStoreCatalog(app);
        
        Console.WriteLine("\nLook for watches...");
        var watchData = await app.ExecuteQuery<FindProductQuery, Product>(new("watch"));
        Console.WriteLine(watchData);

        Console.WriteLine("... and buy 10 of them.");
        await app.ExecuteCommand<BuyProductCommand>(new(watchData.Id, 10));

        Console.WriteLine("\nNow the store catalog looks like this:");
        await PrintStoreCatalog(app);
    }

    private static async Task PrintStoreCatalog(IModule app)
    {
        var products = await app.ExecuteQuery<GetProductsQuery, IEnumerable<Product>>(new());
        foreach (var product in products)
        {
            Console.WriteLine(product);
        }
    }
}