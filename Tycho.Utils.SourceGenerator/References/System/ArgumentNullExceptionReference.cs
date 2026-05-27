using System;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class ArgumentNullExceptionReference
    {
        public static TypeModel TypeModel { get; } = new TypeModel(typeof(ArgumentNullException).Namespace, nameof(ArgumentNullException));
    }
}
