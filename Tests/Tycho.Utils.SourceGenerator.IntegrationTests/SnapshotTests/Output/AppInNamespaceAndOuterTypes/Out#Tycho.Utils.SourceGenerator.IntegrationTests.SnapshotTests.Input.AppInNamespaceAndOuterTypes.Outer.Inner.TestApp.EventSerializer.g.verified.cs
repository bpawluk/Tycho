//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInNamespaceAndOuterTypes.Outer.Inner.TestApp.EventSerializer.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInNamespaceAndOuterTypes
{
    public partial class Outer
    {
        public partial class Inner
        {
            internal class TestAppEventSerializer : global::Tycho.Events.Serialization.EventSerializerBase
            {
                public TestAppEventSerializer(global::Tycho.Events.Serialization.IPayloadSerializer payloadSerializer) : base(payloadSerializer)
                {
                }
            }
        }
    }
}
