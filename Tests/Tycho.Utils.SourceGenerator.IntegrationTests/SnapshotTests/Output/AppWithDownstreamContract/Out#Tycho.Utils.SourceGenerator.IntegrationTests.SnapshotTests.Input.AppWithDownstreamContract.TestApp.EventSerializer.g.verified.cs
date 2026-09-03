//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.TestApp.EventSerializer.g.cs
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract
{
    internal class TestAppEventSerializer : EventSerializerBase
    {
        public TestAppEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
        {
        }
    }
}
