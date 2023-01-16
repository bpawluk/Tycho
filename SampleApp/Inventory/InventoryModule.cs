using Microsoft.Extensions.DependencyInjection;
using SampleApp.Inventory.Persistence;
using System;
using Tycho;
using Tycho.Contract;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace SampleApp.Inventory;

// Incoming
public record ReserveProductCommand(string ProductId, int Amount) : ICommand;

// Outgoing
public record StockLevelChangedEvent(string ProductId, int PreviousLevel, int CurrentLevel) : IEvent;

public class InventoryModule : TychoModule
{
    protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        module.Executes<ReserveProductCommand>(commandData => 
        {
            services.GetService<IStockLevelsRepository>()!
                    .ReserveProduct(commandData.ProductId, commandData.Amount);
        });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Publishes<StockLevelChangedEvent>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition submodules, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IStockLevelsRepository, StockLevelsRepository>();
    }
}
