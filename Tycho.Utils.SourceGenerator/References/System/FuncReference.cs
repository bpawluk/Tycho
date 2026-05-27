using System;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class FuncReference
    {
        public static TypeReferenceModel TypeModel { get; } = new TypeReferenceModel(typeof(Func<object>).Namespace, nameof(Func<object>));
    }
}
