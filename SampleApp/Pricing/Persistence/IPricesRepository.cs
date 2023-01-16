namespace SampleApp.Pricing.Data;

internal interface IPricesRepository
{
    decimal GetPriceByProductId(string productId);
    void SetPrice(string productId, decimal price);
}
