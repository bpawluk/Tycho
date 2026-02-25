using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References
{
    internal static class IPayloadSerializerReference
    {
        private const string _namespace = "Tycho.Events.Serialization";
        private const string _typeName = "IPayloadSerializer";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);

        public static MethodSignatureModel DeserializeMethodSignature => new MethodSignatureModel(
            methodName: "Deserialize",
            parameters: new ImmutableEquatableArray<TypeModel>(new[]
            {
                ObjectReference.TypeModel,
            }),
            result: ObjectReference.TypeModel);
    }
}
