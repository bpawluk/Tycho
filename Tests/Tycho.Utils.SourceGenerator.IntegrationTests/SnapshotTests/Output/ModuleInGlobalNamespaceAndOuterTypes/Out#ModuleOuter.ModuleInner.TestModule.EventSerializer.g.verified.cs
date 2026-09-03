//HintName: ModuleOuter.ModuleInner.TestModule.EventSerializer.g.cs
using Tycho.Events.Serialization;

public partial class ModuleOuter
{
    public partial class ModuleInner
    {
        internal class TestModuleEventSerializer : EventSerializerBase
        {
            public TestModuleEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
            {
            }
        }
    }
}
