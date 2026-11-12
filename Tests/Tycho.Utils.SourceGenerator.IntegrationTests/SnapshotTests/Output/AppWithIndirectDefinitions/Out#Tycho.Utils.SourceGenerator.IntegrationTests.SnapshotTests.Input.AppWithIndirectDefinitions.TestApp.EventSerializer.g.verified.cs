//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestApp.EventSerializer.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions
{
    internal class TestAppEventSerializer : global::Tycho.Events.Serialization.EventSerializerBase
    {
        public TestAppEventSerializer(global::Tycho.Events.Serialization.IPayloadSerializer payloadSerializer) : base(payloadSerializer)
        {
            RegisterEvent<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromHelperExtension>();
            RegisterEvent<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromHelperStaticClass>();
            RegisterEvent<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromHelperClass>();
            RegisterEvent<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromLocalStaticHelper>();
            RegisterEvent<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromLocalHelper>();
        }
    }
}
