using System;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class FuncReference
    {
        public static TypeModel TypeModel { get; } = new TypeModel(typeof(Func<object>).Namespace, nameof(Func<object>));
    }
}
