//HintName: TestApp.EventSerializer.g.cs
using Tycho.Events.Serialization;

internal class TestAppEventSerializer : EventSerializerBase
{
    public TestAppEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
    {
    }
}
