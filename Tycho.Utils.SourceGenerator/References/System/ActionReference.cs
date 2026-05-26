using System;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class ActionReference
    {
        public static TypeModel TypeModel { get; } = new TypeModel(typeof(Action).Namespace, nameof(Action));
    }
}
