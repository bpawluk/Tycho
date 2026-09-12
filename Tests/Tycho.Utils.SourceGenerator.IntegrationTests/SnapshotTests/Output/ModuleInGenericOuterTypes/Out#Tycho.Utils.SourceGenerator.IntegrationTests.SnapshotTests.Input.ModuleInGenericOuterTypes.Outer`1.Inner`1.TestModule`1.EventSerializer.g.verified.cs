//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleInGenericOuterTypes.Outer`1.Inner`1.TestModule`1.EventSerializer.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            internal class TestModuleEventSerializer<TModule> : global::Tycho.Events.Serialization.EventSerializerBase
                where TModule : notnull
            {
                public TestModuleEventSerializer(global::Tycho.Events.Serialization.IPayloadSerializer payloadSerializer) : base(payloadSerializer)
                {
                }
            }
        }
    }
}
