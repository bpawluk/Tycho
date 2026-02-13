using System;
using Tycho.Utils.SourceGenerator.Model.Partial;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class FuncReference
    {
        public static TypeModel NoParamTypeModel { get; } = new TypeModel(
            typeof(Func<object>).Namespace, 
            ImmutableEquatableArray<string>.Empty, 
            typeof(Func<object>).Name);

        public static TypeModel OneParamTypeModel { get; } = new TypeModel(
            typeof(Func<object, object>).Namespace,
            ImmutableEquatableArray<string>.Empty,
            typeof(Func<object, object>).Name);
    }
}
