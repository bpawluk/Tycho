//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppsWithSameNestedName.Beta.TestApp.EventSerializer.g.cs
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppsWithSameNestedName
{
    public partial class Beta
    {
        internal class TestAppEventSerializer : EventSerializerBase
        {
            public TestAppEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
            {
            }
        }
    }
}
