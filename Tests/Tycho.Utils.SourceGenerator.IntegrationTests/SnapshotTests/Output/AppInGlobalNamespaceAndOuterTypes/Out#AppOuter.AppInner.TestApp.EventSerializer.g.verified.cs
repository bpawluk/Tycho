//HintName: AppOuter.AppInner.TestApp.EventSerializer.g.cs
public partial class AppOuter
{
    public partial class AppInner
    {
        internal class TestAppEventSerializer : global::Tycho.Events.Serialization.EventSerializerBase
        {
            public TestAppEventSerializer(global::Tycho.Events.Serialization.IPayloadSerializer payloadSerializer) : base(payloadSerializer)
            {
            }
        }
    }
}
