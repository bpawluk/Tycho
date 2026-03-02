using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Events
{
    internal static class IEventDispatcherReference
    {
        private const string _namespace = "Tycho.Events.Dispatching";
        private const string _typeName = "IEventDispatcher";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);
    }
}
