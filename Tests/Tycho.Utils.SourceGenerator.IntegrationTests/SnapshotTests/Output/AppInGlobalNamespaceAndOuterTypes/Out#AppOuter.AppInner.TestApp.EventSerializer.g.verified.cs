//HintName: AppOuter.AppInner.TestApp.EventSerializer.g.cs
using Tycho.Events.Serialization;

public partial class AppOuter
{
    public partial class AppInner
    {
        internal class TestAppEventSerializer : EventSerializerBase
        {
            public TestAppEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
            {
            }
        }
    }
}
