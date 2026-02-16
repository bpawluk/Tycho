using System;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class GuidReference
    {
        public static TypeModel TypeModel { get; } = new TypeModel(typeof(Guid).Namespace, ImmutableEquatableArray<string>.Empty, nameof(Guid));
    }
}
