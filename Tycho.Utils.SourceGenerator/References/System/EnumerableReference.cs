using System;
using System.Collections.Generic;
using System.Linq;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class EnumerableReference
    {
        private const string SourceTypeParameterName = "TSource";

        public static TypeReferenceModel TypeModel { get; } = new TypeReferenceModel(
            typeof(Enumerable).Namespace,
            nameof(Enumerable));

        public static MethodSignatureModel AnyMethodSignature => new MethodSignatureModel(
            methodName: nameof(Enumerable.Any),
            parameters: new ImmutableEquatableArray<TypeReferenceModel>(new[]
            {
                CreateEnumerableTypeModel(),
                CreatePredicateTypeModel(),
            }),
            result: BooleanReference.TypeModel);

        private static TypeReferenceModel CreateEnumerableTypeModel()
        {
            TypeReferenceModel sourceType = CreateSourceTypeModel();
            return new TypeReferenceModel(
                typeof(IEnumerable<>).Namespace,
                ImmutableEquatableArray<TypeReferenceModel>.Empty,
                nameof(IEnumerable<object>),
                new ImmutableEquatableArray<TypeArgumentModel>(new[]
                {
                    new TypeArgumentModel(SourceTypeParameterName, sourceType),
                }));
        }

        private static TypeReferenceModel CreatePredicateTypeModel()
        {
            TypeReferenceModel sourceType = CreateSourceTypeModel();
            return new TypeReferenceModel(
                typeof(Func<,>).Namespace,
                ImmutableEquatableArray<TypeReferenceModel>.Empty,
                nameof(Func<object, bool>),
                new ImmutableEquatableArray<TypeArgumentModel>(new[]
                {
                    new TypeArgumentModel(SourceTypeParameterName, sourceType),
                    new TypeArgumentModel(nameof(Boolean), BooleanReference.TypeModel),
                }));
        }

        private static TypeReferenceModel CreateSourceTypeModel() => new TypeReferenceModel(
            string.Empty,
            SourceTypeParameterName);
    }
}
