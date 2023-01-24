using Tycho.Messaging.Payload;

namespace SampleApp.App.Contract
{
    public record BuyProductCommand(string ProductId, int Amount) : ICommand;
}
