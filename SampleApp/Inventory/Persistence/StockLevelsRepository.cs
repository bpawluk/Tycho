using System;
using System.Collections.Generic;
using Tycho;

namespace SampleApp.Inventory.Persistence;

internal class StockLevelsRepository : IStockLevelsRepository
{
    private readonly IModule _thisModule;
    private readonly Dictionary<string, int> _stockLevelData = new()
    {
        ["product-01"] = 100,
        ["product-02"] = 100,
        ["product-03"] = 100,
    };

    public StockLevelsRepository(IModule thisModule) 
    {
        _thisModule = thisModule;
    }

    public void ReserveProduct(string productId, int amount)
    {
        if (!_stockLevelData.ContainsKey(productId))
        {
            throw new ArgumentException($"There is no stock level data for product {productId}");
        }

        var previousLevel = _stockLevelData[productId];
        var newLevel = previousLevel - amount;
        _stockLevelData[productId] = newLevel;

        _thisModule.PublishEvent(new StockLevelChangedEvent(productId, previousLevel, newLevel));
    }
}
