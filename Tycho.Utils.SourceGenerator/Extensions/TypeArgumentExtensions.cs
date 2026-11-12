using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Apps;
using Tycho.Utils.SourceGenerator.References.Tycho.Modules;

namespace Tycho.Utils.SourceGenerator.Extensions
{
    internal static class TypeArgumentExtensions
    {
        public static bool IsRequestType(this TypeArgumentModel typeArgument) =>
            typeArgument.Name == IAppContractReference.RequestTypeParameterName ||
            typeArgument.Name == IModuleContractReference.RequestTypeParameterName;

        public static bool IsResponseType(this TypeArgumentModel typeArgument) =>
            typeArgument.Name == IAppContractReference.ResponseTypeParameterName ||
            typeArgument.Name == IModuleContractReference.ResponseTypeParameterName;

        public static bool IsEventType(this TypeArgumentModel typeArgument) =>
            typeArgument.Name == IAppEventsReference.EventTypeParameterName ||
            typeArgument.Name == IModuleEventsReference.EventTypeParameterName;

        public static bool IsModuleType(this TypeArgumentModel typeArgument) =>
            typeArgument.Name == IAppStructureReference.ModuleTypeParameterName ||
            typeArgument.Name == IModuleStructureReference.ModuleTypeParameterName;
    }
}
