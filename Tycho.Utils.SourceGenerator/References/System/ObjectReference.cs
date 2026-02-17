using System;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal class ObjectReference
    {
        public static TypeModel TypeModel { get; } = new TypeModel(typeof(object).Namespace, ImmutableEquatableArray<string>.Empty, nameof(Object));
    }
}
