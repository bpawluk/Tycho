using Tycho.Utils.SourceGenerator.Model.Generic;
using Tycho.Utils.SourceGenerator.Model.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class ActionReference
    {
        public static TypeModel TypeModel { get; } = new TypeModel("System", ImmutableEquatableArray<string>.Empty, "Action");
    }
}
