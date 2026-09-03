//HintName: TestModule.EventSerializer.g.cs
using Tycho.Events.Serialization;

internal class TestModuleEventSerializer : EventSerializerBase
{
    public TestModuleEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
    {
    }
}
