using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Apps;
using Tycho.Utils.SourceGenerator.References.Tycho.Modules;

namespace Tycho.Utils.SourceGenerator.Extensions
{
    internal static class MethodSignatureModelExtensions
    {
        public static bool IsDefineContractMethod(this MethodSignatureModel methodSignature) =>
            TychoAppReference.DefineContractMethodSignature.Equals(methodSignature) ||
            TychoModuleReference.DefineContractMethodSignature.Equals(methodSignature);

        public static bool IsDownstreamContractDefiningMethod(this MethodSignatureModel methodSignature) =>
            IAppContractReference.DownstreamContractDefiningMethods.Contains(methodSignature) ||
            IModuleContractReference.DownstreamContractDefiningMethods.Contains(methodSignature);

        public static bool IsUpstreamContractDefiningMethod(this MethodSignatureModel methodSignature) =>
            IModuleContractReference.UpstreamContractDefiningMethods.Contains(methodSignature);

        public static bool IsDefineEventsMethod(this MethodSignatureModel methodSignature) =>
            TychoAppReference.DefineEventsMethodSignature.Equals(methodSignature) ||
            TychoModuleReference.DefineEventsMethodSignature.Equals(methodSignature);

        public static bool IsHandledEventDefiningMethod(this MethodSignatureModel methodSignature) =>
            IAppEventsReference.HandledEventDefiningMethods.Contains(methodSignature) ||
            IModuleEventsReference.HandledEventDefiningMethods.Contains(methodSignature);

        public static bool IsHandledOrRoutedEventDefiningMethod(this MethodSignatureModel methodSignature) =>
            IAppEventsReference.HandledOrRoutedEventDefiningMethods.Contains(methodSignature) ||
            IModuleEventsReference.HandledOrRoutedEventDefiningMethods.Contains(methodSignature);

        public static bool IsIncludeModulesMethod(this MethodSignatureModel methodSignature) =>
            TychoAppReference.IncludeModulesMethodSignature.Equals(methodSignature) ||
            TychoModuleReference.IncludeModulesMethodSignature.Equals(methodSignature);

        public static bool IsSubmoduleDefiningMethod(this MethodSignatureModel methodSignature) =>
            IAppStructureReference.SubmoduleDefiningMethods.Contains(methodSignature) ||
            IModuleStructureReference.SubmoduleDefiningMethods.Contains(methodSignature);
    }
}
