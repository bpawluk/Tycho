using System;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Routing.Routes;
using Tycho.Structure.Internal;

namespace Tycho.Events.Routing.Delivery
{
    internal interface IDeliveryStrategyProvider
    {
        IDeliveryStrategy GetDeliveryStrategy(IRouteStep routeStep);
    }

    internal class DeliveryStrategyProvider : IDeliveryStrategyProvider
    {
        private readonly Internals _internals;

        public DeliveryStrategyProvider(Internals internals)
        {
            _internals = internals;
        }

        public IDeliveryStrategy GetDeliveryStrategy(IRouteStep routeStep)
        {
            return routeStep switch
            {
                FinalRouteStep _ => _internals.GetRequiredService<DownStreamRouteDelivery>(),
                DownStreamRouteStep _ => _internals.GetRequiredService<DownStreamRouteDelivery>(),
                UpStreamRouteStep _ => _internals.GetRequiredService<UpStreamRouteDelivery>(),
                _ => throw new InvalidOperationException($"No delivery strategy defined for {routeStep.GetType().Name}."),
            };
        }
    }
}
