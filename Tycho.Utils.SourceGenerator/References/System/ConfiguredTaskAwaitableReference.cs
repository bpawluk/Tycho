using System.Runtime.CompilerServices;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class ConfiguredTaskAwaitableReference
    {
        public static TypeModel TypeModel { get; } = new TypeModel(typeof(ConfiguredTaskAwaitable).Namespace, nameof(ConfiguredTaskAwaitable));
    }
}
