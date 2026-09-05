//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents.TestApp.EventSerializer.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents
{
    internal class TestAppEventSerializer : global::Tycho.Events.Serialization.EventSerializerBase
    {
        public TestAppEventSerializer(global::Tycho.Events.Serialization.IPayloadSerializer payloadSerializer) : base(payloadSerializer)
        {
            RegisterEvent<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents.Events.OrderCreatedEvent>();
            RegisterEvent<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents.Events.PaymentProcessedEvent>();
        }
    }
}
