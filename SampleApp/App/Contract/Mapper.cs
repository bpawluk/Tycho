namespace SampleApp.App.Contract
{
    internal class Mapper
    {
        public static Catalog.PriceChangedEvent MapToCatalog(Pricing.PriceChangedEvent eventData)
        {
            return new Catalog.PriceChangedEvent(eventData.ProductId, eventData.OldPrice, eventData.NewPrice);
        }

        public static Pricing.StockLevelChangedEvent MapToPricing(Inventory.StockLevelChangedEvent eventData)
        {
            return new Pricing.StockLevelChangedEvent(
                eventData.ProductId,
                eventData.PreviousLevel,
                eventData.CurrentLevel);
        }

        public static Catalog.StockLevelChangedEvent MapToCatalog(Inventory.StockLevelChangedEvent eventData)
        {
            return new Catalog.StockLevelChangedEvent(
                eventData.ProductId,
                eventData.PreviousLevel,
                eventData.CurrentLevel);
        }
    }
}
