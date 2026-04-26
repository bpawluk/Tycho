//HintName: Outer.Inner.TestModule.EventSerializer.g.cs
using Tycho.Events.Serialization;

public partial class Outer
{
    public partial class Inner
    {
        internal class TestModuleEventSerializer : EventSerializerBase
        {
            public TestModuleEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
            {
            }
        }
    }
}
