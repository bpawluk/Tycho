//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.TestModule.EventSerializer.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents
{
    internal class TestModuleEventSerializer : global::Tycho.Events.Serialization.EventSerializerBase
    {
        public TestModuleEventSerializer(global::Tycho.Events.Serialization.IPayloadSerializer payloadSerializer) : base(payloadSerializer)
        {
            RegisterEvent<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Events.OrderCreatedEvent>();
            RegisterEvent<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Events.PaymentProcessedEvent>();
        }
    }
}
