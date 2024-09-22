using Microsoft.Extensions.DependencyInjection;
using NewTycho.Structure;
using System.Collections.Generic;

namespace NewTycho.Messaging
{
    internal class LocalHandlersProvider<THandler> : IHandlersProvider
        where THandler : class, IHandle
    {
        private readonly ModuleInternals _moduleInternals;
        private readonly string _key;

        public LocalHandlersProvider(ModuleInternals moduleInternals, string key)
        {
            _moduleInternals = moduleInternals;
            _key = key;
            _moduleInternals.GetServiceCollection().AddKeyedTransient<THandler>(_key);
        }

        public IReadOnlyList<string> IdentifyHandlers() => new string[] { typeof(THandler).FullName };

        public IReadOnlyList<IHandle> GetHandlers() => new IHandle[] { _moduleInternals.GetKeyedService<THandler>(_key)! };
    }
}
