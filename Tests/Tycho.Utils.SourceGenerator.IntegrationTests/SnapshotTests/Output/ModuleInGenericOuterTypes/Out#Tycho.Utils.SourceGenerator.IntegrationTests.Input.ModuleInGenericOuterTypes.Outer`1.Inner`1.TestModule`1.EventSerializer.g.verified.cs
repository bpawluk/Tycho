//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInGenericOuterTypes.Outer`1.Inner`1.TestModule`1.EventSerializer.g.cs
using Tycho.Events.Serialization;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            internal class TestModuleEventSerializer<TModule> : EventSerializerBase
                where TModule : notnull
            {
                public TestModuleEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
                {
                }
            }
        }
    }
}
