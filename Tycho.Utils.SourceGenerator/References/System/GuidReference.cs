using System;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class GuidReference
    {
        public static TypeModel TypeModel { get; } = new TypeModel(typeof(Guid).Namespace, nameof(Guid));
    }
}
