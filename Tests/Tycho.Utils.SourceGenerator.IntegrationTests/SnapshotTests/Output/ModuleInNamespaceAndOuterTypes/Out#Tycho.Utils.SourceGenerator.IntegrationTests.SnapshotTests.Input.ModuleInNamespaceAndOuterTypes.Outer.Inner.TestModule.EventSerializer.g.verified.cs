//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleInNamespaceAndOuterTypes.Outer.Inner.TestModule.EventSerializer.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleInNamespaceAndOuterTypes
{
    public partial class Outer
    {
        public partial class Inner
        {
            internal class TestModuleEventSerializer : global::Tycho.Events.Serialization.EventSerializerBase
            {
                public TestModuleEventSerializer(global::Tycho.Events.Serialization.IPayloadSerializer payloadSerializer) : base(payloadSerializer)
                {
                }
            }
        }
    }
}
