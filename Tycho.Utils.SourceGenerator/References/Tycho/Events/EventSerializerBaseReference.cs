using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Events
{
    internal static class EventSerializerBaseReference
    {
        private const string _namespace = "Tycho.Events.Serialization";
        private const string _typeName = "EventSerializerBase";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);

        public static MethodSignatureModel RegisterEventMethodSignature => new MethodSignatureModel(
            methodName: "RegisterEvent",
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: VoidReference.TypeModel);
    }
}