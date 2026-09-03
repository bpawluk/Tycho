//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithEvents.TestApp.EventSerializer.g.cs
using Tycho.Events.Serialization;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithEvents.Events;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithEvents
{
    internal class TestAppEventSerializer : EventSerializerBase
    {
        public TestAppEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
        {
            RegisterEvent<OrderCreatedEvent>();
            RegisterEvent<PaymentProcessedEvent>();
        }
    }
}
