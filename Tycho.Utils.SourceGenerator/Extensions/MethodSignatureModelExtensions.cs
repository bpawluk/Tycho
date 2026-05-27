using System.Linq;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Apps;
using Tycho.Utils.SourceGenerator.References.Tycho.Modules;

namespace Tycho.Utils.SourceGenerator.Extensions
{
    internal static class MethodSignatureModelExtensions
    {
        public static bool IsDefineContractMethod(this MethodSignatureModel methodSignature) =>
            TychoAppReference.DefineContractMethodSignature.Matches(methodSignature) ||
            TychoModuleReference.DefineContractMethodSignature.Matches(methodSignature);

        public static bool IsDownstreamContractDefiningMethod(this MethodSignatureModel methodSignature) =>
            IAppContractReference.DownstreamContractDefiningMethods.Any(m => m.Matches(methodSignature)) ||
            IModuleContractReference.DownstreamContractDefiningMethods.Any(m => m.Matches(methodSignature));

        public static bool IsUpstreamContractDefiningMethod(this MethodSignatureModel methodSignature) =>
            IModuleContractReference.UpstreamContractDefiningMethods.Any(m => m.Matches(methodSignature));
        public static bool IsDefineEventsMethod(this MethodSignatureModel methodSignature) =>
            TychoAppReference.DefineEventsMethodSignature.Matches(methodSignature) ||
            TychoModuleReference.DefineEventsMethodSignature.Matches(methodSignature);

        public static bool IsHandledEventDefiningMethod(this MethodSignatureModel methodSignature) =>
            IAppEventsReference.HandledEventDefiningMethods.Any(m => m.Matches(methodSignature)) ||
            IModuleEventsReference.HandledEventDefiningMethods.Any(m => m.Matches(methodSignature));

        public static bool IsHandledOrRoutedEventDefiningMethod(this MethodSignatureModel methodSignature) =>
            IAppEventsReference.HandledOrRoutedEventDefiningMethods.Any(m => m.Matches(methodSignature)) ||
            IModuleEventsReference.HandledOrRoutedEventDefiningMethods.Any(m => m.Matches(methodSignature));

        public static bool IsIncludeModulesMethod(this MethodSignatureModel methodSignature) =>
            TychoAppReference.IncludeModulesMethodSignature.Matches(methodSignature) ||
            TychoModuleReference.IncludeModulesMethodSignature.Matches(methodSignature);

        public static bool IsSubmoduleDefiningMethod(this MethodSignatureModel methodSignature) =>
            IAppStructureReference.SubmoduleDefiningMethods.Any(m => m.Matches(methodSignature)) ||
            IModuleStructureReference.SubmoduleDefiningMethods.Any(m => m.Matches(methodSignature));
    }
}
