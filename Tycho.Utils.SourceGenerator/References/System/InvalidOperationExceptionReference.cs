using System;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class InvalidOperationExceptionReference
    {
        public static TypeModel TypeModel { get; } = new TypeModel(typeof(InvalidOperationException).Namespace,nameof(InvalidOperationException));
    }
}
