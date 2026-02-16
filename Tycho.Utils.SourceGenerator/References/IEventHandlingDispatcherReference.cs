using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References
{
    internal static class IEventHandlingDispatcherReference
    {
        private const string _namespace = "Tycho.Events.Handling";
        private const string _typeName = "IEventHandlingDispatcher";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);
    }
}
