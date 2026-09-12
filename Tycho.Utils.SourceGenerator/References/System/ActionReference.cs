using System;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class ActionReference
    {
        public static TypeReferenceModel CreateTypeModel(TypeReferenceModel argument) => new TypeReferenceModel(
            typeof(Action).Namespace,
            ImmutableEquatableArray<TypeReferenceModel>.Empty,
            nameof(Action),
            new ImmutableEquatableArray<TypeArgumentModel>(new[]
            {
                new TypeArgumentModel("T", argument),
            }));
    }
}
