using NewTycho.Events.Outbox;
using NewTycho.Requests.Outbox;
using System;

namespace NewTycho.Modules.Contract
{
    internal class ModuleOutboxBuilder : IModuleOutbox, IOutboxConsumer
    {
        IEventOutboxDefinition IModuleOutbox.Events => throw new NotImplementedException();

        IRequestOutboxDefinition IModuleOutbox.Requests => RequestOutboxBuilder;

        IEventOutboxConsumer IOutboxConsumer.Events => throw new NotImplementedException();

        IRequestOutboxConsumer IOutboxConsumer.Requests => RequestOutboxBuilder;

        public RequestOutboxBuilder RequestOutboxBuilder { get; }

        public ModuleOutboxBuilder(IServiceProvider? externalServices)
        {
            RequestOutboxBuilder = new RequestOutboxBuilder();
        }
    }
}
