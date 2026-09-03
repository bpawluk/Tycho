//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInNamespaceAndOuterTypes.Outer.Inner.TestApp.EventSerializer.g.cs
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInNamespaceAndOuterTypes
{
    public partial class Outer
    {
        public partial class Inner
        {
            internal class TestAppEventSerializer : EventSerializerBase
            {
                public TestAppEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
                {
                }
            }
        }
    }
}
