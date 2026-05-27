using System;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal class ObjectReference
    {
        public static TypeReferenceModel TypeModel { get; } = new TypeReferenceModel(typeof(object).Namespace, nameof(Object));

        public static MethodSignatureModel GetTypeMethodSignature => new MethodSignatureModel(
            methodName: nameof(GetType),
            parameters: ImmutableEquatableArray<TypeReferenceModel>.Empty,
            result: TypeReference.TypeModel);
    }
}
