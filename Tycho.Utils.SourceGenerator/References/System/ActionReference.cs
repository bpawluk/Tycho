using System;
using Tycho.Utils.SourceGenerator.Model.Partial;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class ActionReference
    {
        public static TypeModel NoParamTypeModel { get; } = new TypeModel(
            typeof(Action).Namespace, 
            ImmutableEquatableArray<string>.Empty, 
            typeof(Action<object>).Name);

        public static TypeModel OneParamTypeModel { get; } = new TypeModel(
            typeof(Action<object>).Namespace,
            ImmutableEquatableArray<string>.Empty,
            typeof(Action<object>).Name);
    }
}
