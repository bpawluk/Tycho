using System;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class ActionReference
    {
        public static TypeReferenceModel TypeModel { get; } = new TypeReferenceModel(typeof(Action).Namespace, nameof(Action));
    }
}
