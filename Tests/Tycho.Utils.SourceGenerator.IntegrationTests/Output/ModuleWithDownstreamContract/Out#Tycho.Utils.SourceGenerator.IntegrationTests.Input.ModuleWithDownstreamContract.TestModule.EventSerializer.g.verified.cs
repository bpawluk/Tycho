//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithDownstreamContract.TestModule.EventSerializer.g.cs
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithDownstreamContract
{
    internal class TestModuleEventSerializer : EventSerializerBase
    {
        public TestModuleEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
        {
        }
    }
}
