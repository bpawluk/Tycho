using System;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal class ObjectReference
    {
        public static TypeModel TypeModel { get; } = new TypeModel(typeof(object).Namespace, ImmutableEquatableArray<string>.Empty, nameof(Object));

        public static MethodSignatureModel GetTypeMethodSignature => new MethodSignatureModel(
            methodName: nameof(GetType),
            parameters: ImmutableEquatableArray<TypeModel>.Empty,
            result: TypeReference.TypeModel);
    }
}
