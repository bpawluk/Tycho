//HintName: Outer.Inner.TestApp.EventSerializer.g.cs
using Tycho.Events.Serialization;

public partial class Outer
{
    public partial class Inner
    {
        internal class TestAppEventSerializer : EventSerializerBase
        {
            public TestAppEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
            {
            }
        }
    }
}
