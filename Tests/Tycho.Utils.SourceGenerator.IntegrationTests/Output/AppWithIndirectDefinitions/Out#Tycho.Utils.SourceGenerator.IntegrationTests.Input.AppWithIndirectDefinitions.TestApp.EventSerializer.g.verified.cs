//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.TestApp.EventSerializer.g.cs
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions
{
    internal class TestAppEventSerializer : EventSerializerBase
    {
        public TestAppEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
        {
            RegisterEvent<TestEventFromHelperExtension>();
            RegisterEvent<TestEventFromHelperStaticClass>();
            RegisterEvent<TestEventFromHelperClass>();
            RegisterEvent<TestEventFromLocalStaticHelper>();
            RegisterEvent<TestEventFromLocalHelper>();
        }
    }
}
