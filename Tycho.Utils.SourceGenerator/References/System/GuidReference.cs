using System;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class GuidReference
    {
        public static TypeReferenceModel TypeModel { get; } = new TypeReferenceModel(typeof(Guid).Namespace, nameof(Guid));
    }
}
