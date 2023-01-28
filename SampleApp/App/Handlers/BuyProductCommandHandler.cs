using SampleApp.App.Contract;
using SampleApp.Inventory;
using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace SampleApp.App.Handlers;

internal class BuyProductCommandHandler : ICommandHandler<BuyProductCommand>
{
    private readonly IModule<InventoryModule> _inventoryModule;

    public BuyProductCommandHandler(IModule<InventoryModule> inventoryModule)
    {
        _inventoryModule = inventoryModule;
    }

    public Task Handle(BuyProductCommand commandData, CancellationToken cancellationToken)
    {
        return _inventoryModule.Execute(
            new ReserveProductCommand(
                commandData.ProductId, 
                commandData.Amount), 
            cancellationToken);
    }
}
