using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Apps;
using Tycho.Utils.SourceGenerator.References.Tycho.Modules;

namespace Tycho.Utils.SourceGenerator.Extensions
{
    internal static class MethodInvocationModelExtensions
    {
        public static bool HasStructureReceiver(this MethodInvocationModel invocation) =>
            invocation.ReceiverType.HasValue &&
            (invocation.ReceiverType.Value.Matches(IAppStructureReference.TypeModel) ||
             invocation.ReceiverType.Value.Matches(IModuleStructureReference.TypeModel));
    }
}
