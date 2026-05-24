//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition.TestApp`2.EventSerializer.g.cs
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition
{
    internal class TestAppEventSerializer<TPayload, TKey> : EventSerializerBase
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
        public TestAppEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
        {
        }
    }
}
