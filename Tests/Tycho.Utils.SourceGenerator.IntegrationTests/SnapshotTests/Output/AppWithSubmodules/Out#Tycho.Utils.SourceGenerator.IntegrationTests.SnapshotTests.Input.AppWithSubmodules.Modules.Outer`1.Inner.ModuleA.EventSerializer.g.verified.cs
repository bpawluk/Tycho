//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules.Modules.Outer`1.Inner.ModuleA.EventSerializer.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules.Modules
{
    public partial class Outer<TOuter>
    {
        public partial class Inner
        {
            internal class ModuleAEventSerializer : global::Tycho.Events.Serialization.EventSerializerBase
            {
                public ModuleAEventSerializer(global::Tycho.Events.Serialization.IPayloadSerializer payloadSerializer) : base(payloadSerializer)
                {
                }
            }
        }
    }
}
