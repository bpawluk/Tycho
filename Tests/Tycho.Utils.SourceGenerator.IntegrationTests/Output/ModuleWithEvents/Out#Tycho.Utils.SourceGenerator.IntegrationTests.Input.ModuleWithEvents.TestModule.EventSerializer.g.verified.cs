//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents.TestModule.EventSerializer.g.cs
using Tycho.Events.Serialization;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents.Events;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents
{
    internal class TestModuleEventSerializer : EventSerializerBase
    {
        public TestModuleEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
        {
            RegisterEvent<OrderCreatedEvent>();
            RegisterEvent<PaymentProcessedEvent>();
            RegisterEvent<PaymentFailedEvent>();
        
        }
    }
}
