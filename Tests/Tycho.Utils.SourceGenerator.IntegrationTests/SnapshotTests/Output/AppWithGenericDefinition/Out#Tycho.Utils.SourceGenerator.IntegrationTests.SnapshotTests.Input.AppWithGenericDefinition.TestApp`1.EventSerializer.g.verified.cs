//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithGenericDefinition.TestApp`1.EventSerializer.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithGenericDefinition
{
    internal class TestAppEventSerializer<T> : global::Tycho.Events.Serialization.EventSerializerBase
    {
        public TestAppEventSerializer(global::Tycho.Events.Serialization.IPayloadSerializer payloadSerializer) : base(payloadSerializer)
        {
        }
    }
}
