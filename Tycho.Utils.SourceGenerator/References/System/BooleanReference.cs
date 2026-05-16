using System;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class BooleanReference
    {
        public static TypeModel TypeModel { get; } = new TypeModel(typeof(bool).Namespace, ImmutableEquatableArray<string>.Empty, nameof(Boolean));
    }
}
