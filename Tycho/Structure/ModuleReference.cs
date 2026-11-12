using System;
using Tycho.Events.Delivery;

namespace Tycho.Structure
{
    internal sealed class ModuleReference
    {
        public IDeliveryEndpoint Endpoint { get; }

        public ModuleReference(IDeliveryEndpoint endpoint)
        {
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }
    }
}
