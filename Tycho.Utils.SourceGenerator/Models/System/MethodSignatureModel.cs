using System;
using Tycho.Utils.SourceGenerator.References.Tycho.Apps;
using Tycho.Utils.SourceGenerator.References.Tycho.Modules;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models.System
{
    public readonly struct MethodSignatureModel : IEquatable<MethodSignatureModel>
    {
        public string MethodName { get; }

        public ImmutableEquatableArray<TypeModel> Parameters { get; }

        public TypeModel Result { get; }

        public bool IsDefineContractMethod =>
            TychoAppReference.DefineContractMethodSignature.Equals(this) ||
            TychoModuleReference.DefineContractMethodSignature.Equals(this);

        public bool IsContractDefiningMethod =>
            IAppContractReference.ContractDefiningMethods.Contains(this) ||
            IModuleContractReference.ContractDefiningMethods.Contains(this);

        public bool IsDefineEventsMethod => 
            TychoAppReference.DefineEventsMethodSignature.Equals(this) ||
            TychoModuleReference.DefineEventsMethodSignature.Equals(this);

        public bool IsEventDefiningMethod =>
            IAppEventsReference.EventDefiningMethods.Contains(this) ||
            IModuleEventsReference.EventDefiningMethods.Contains(this);

        public bool IsIncludeModulesMethod =>
            TychoAppReference.IncludeModulesMethodSignature.Equals(this) ||
            TychoModuleReference.IncludeModulesMethodSignature.Equals(this);

        public bool IsSubmoduleDefiningMethod =>
            IAppStructureReference.SubmoduleDefiningMethods.Contains(this) ||
            IModuleStructureReference.SubmoduleDefiningMethods.Contains(this);

        public MethodSignatureModel(
            string methodName,
            ImmutableEquatableArray<TypeModel> parameters,
            TypeModel result)
        {
            MethodName = methodName;
            Parameters = parameters;
            Result = result;
        }

        public bool Equals(MethodSignatureModel other)
        {
            return string.Equals(MethodName, other.MethodName, StringComparison.Ordinal) &&
                   Parameters.Equals(other.Parameters) &&
                   Result.Equals(other.Result);
        }

        public override bool Equals(object obj)
        {
            return obj is MethodSignatureModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                StringComparer.Ordinal.GetHashCode(MethodName ?? string.Empty),
                Parameters.GetHashCode(),
                Result.GetHashCode());
        }

        public static bool operator ==(MethodSignatureModel left, MethodSignatureModel right) => left.Equals(right);

        public static bool operator !=(MethodSignatureModel left, MethodSignatureModel right) => !left.Equals(right);
    }
}
