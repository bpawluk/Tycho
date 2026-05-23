//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.TestApp.EventSerializer.g.cs
using Tycho.Events.Serialization;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions
{
    internal class TestAppEventSerializer : EventSerializerBase
    {
        public TestAppEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
        {
            RegisterEvent<TestEventFromLocalHelper>();
            RegisterEvent<TestEventFromLocalStaticHelper>();
            RegisterEvent<TestEventFromHelperClass>();
            RegisterEvent<TestEventFromHelperStaticClass>();
            RegisterEvent<TestEventFromHelperExtension>();
        }
    }
}
