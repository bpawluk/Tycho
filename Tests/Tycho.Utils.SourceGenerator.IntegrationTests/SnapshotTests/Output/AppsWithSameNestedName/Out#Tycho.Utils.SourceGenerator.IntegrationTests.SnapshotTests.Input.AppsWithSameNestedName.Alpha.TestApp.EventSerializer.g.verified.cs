//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppsWithSameNestedName.Alpha.TestApp.EventSerializer.g.cs
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppsWithSameNestedName
{
    public partial class Alpha
    {
        internal class TestAppEventSerializer : EventSerializerBase
        {
            public TestAppEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
            {
            }
        }
    }
}
