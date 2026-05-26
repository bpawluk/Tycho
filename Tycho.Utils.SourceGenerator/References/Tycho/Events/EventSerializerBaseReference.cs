using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Events
{
    internal static class EventSerializerBaseReference
    {
        private const string Namespace = "Tycho.Events.Serialization";
        private const string TypeName = "EventSerializerBase";

        public static TypeModel TypeModel => new TypeModel(Namespace, TypeName);

        public static MethodSignatureModel RegisterEventMethodSignature => new MethodSignatureModel(
            methodName: "RegisterEvent",
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: VoidReference.TypeModel);
    }
}
