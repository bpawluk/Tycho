using System.Collections.Generic;
using Tycho.Messaging;

namespace Tycho.Structure.Modules
{
    internal class Module : ModuleBase, IModule
    {
        public ModuleInternals Internals { get; }

        public Module()
        {
            Internals = new ModuleInternals();
        }

        public void SetExternalBroker(IMessageBroker broker) => SetMessageBroker(broker);

        public void SetInternalBroker(IMessageBroker broker) => Internals.SetMessageBroker(broker);

        public void SetSubmodules(IEnumerable<IModule> submodules) => Internals.SetSubmodules(submodules);
    }
}
