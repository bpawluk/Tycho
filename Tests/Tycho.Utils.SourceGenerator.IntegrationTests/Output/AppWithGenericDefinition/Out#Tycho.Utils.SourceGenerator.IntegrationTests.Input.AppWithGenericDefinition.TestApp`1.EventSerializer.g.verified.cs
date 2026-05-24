//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithGenericDefinition.TestApp`1.EventSerializer.g.cs
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithGenericDefinition
{
    internal class TestAppEventSerializer<T> : EventSerializerBase
    {
        public TestAppEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
        {
        }
    }
}
