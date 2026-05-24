//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithConstrainedGenericDefinition.TestModule`2.EventSerializer.g.cs
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithConstrainedGenericDefinition
{
    internal class TestModuleEventSerializer<TPayload, TKey> : EventSerializerBase
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
        public TestModuleEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
        {
        }
    }
}
