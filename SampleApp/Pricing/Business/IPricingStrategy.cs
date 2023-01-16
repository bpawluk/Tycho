namespace SampleApp.Pricing.Business;

internal interface IPricingStrategy
{
    void AdjustForAvailability(string productId, int previousStockLevel, int currentStockLevel);
}
