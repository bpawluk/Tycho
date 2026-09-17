using System;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models
{
    internal readonly struct TychoDefinitionModel : IEquatable<TychoDefinitionModel>
    {
        public TychoDefinitionKind DefinitionKind { get; }

        public TypeDefinitionModel DefinitionType { get; }

        public MethodDefinitionModel DefineContractMethod { get; }

        public MethodDefinitionModel DefineEventsMethod { get; }

        public MethodDefinitionModel IncludeModulesMethod { get; }

        public bool IsValid { get; }

        public TychoDefinitionModel(
            TychoDefinitionKind definitionKind,
            TypeDefinitionModel definitionType,
            MethodDefinitionModel defineContractMethod,
            MethodDefinitionModel defineEventsMethod,
            MethodDefinitionModel includeModulesMethod)
        {
            DefinitionKind = definitionKind;
            DefinitionType = definitionType;
            DefineContractMethod = defineContractMethod;
            DefineEventsMethod = defineEventsMethod;
            IncludeModulesMethod = includeModulesMethod;
            IsValid = true;
        }

        public static TychoDefinitionModel None() => new TychoDefinitionModel();

        public bool Equals(TychoDefinitionModel other)
        {
            if (IsValid != other.IsValid)
            {
                return false;
            }

            if (!IsValid) // both are Invalid
            {
                return true;
            }

            return DefinitionKind == other.DefinitionKind &&
                   DefinitionType.Equals(other.DefinitionType) &&
                   DefineContractMethod.Equals(other.DefineContractMethod) &&
                   DefineEventsMethod.Equals(other.DefineEventsMethod) &&
                   IncludeModulesMethod.Equals(other.IncludeModulesMethod) &&
                   IsValid == other.IsValid;
        }

        public override bool Equals(object obj)
        {
            return obj is TychoDefinitionModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            if (!IsValid)
            {
                return 0;
            }

            return HashCode.Combine(
                DefinitionKind.GetHashCode(),
                DefinitionType.GetHashCode(),
                DefineContractMethod.GetHashCode(),
                DefineEventsMethod.GetHashCode(),
                IncludeModulesMethod.GetHashCode(),
                IsValid.GetHashCode());
        }

        public static bool operator ==(TychoDefinitionModel left, TychoDefinitionModel right) => left.Equals(right);

        public static bool operator !=(TychoDefinitionModel left, TychoDefinitionModel right) => !left.Equals(right);
    }
}
