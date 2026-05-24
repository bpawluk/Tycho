using System.Runtime.CompilerServices;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class ConfiguredValueTaskAwaitableReference
    {
        public static TypeModel TypeModel { get; } = new TypeModel(typeof(ConfiguredValueTaskAwaitable).Namespace,nameof(ConfiguredValueTaskAwaitable));
    }
}
