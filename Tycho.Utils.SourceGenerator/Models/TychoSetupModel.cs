using System;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models
{
    public readonly struct TychoSetupModel : IEquatable<TychoSetupModel>
    {
        public TychoDefinitionKind DefinitionKind { get; }

        public TypeDefinitionModel DefinitionType { get; }

        public ImmutableEquatableArray<TypeReferenceModel> Submodules { get; }

        public TychoSetupModel(
            TychoDefinitionKind definitionKind,
            TypeDefinitionModel definitionType,
            ImmutableEquatableArray<TypeReferenceModel> submodules)
        {
            DefinitionKind = definitionKind;
            DefinitionType = definitionType;
            Submodules = submodules;
        }

        public bool Equals(TychoSetupModel other)
        {
            return DefinitionKind.Equals(other.DefinitionKind) &&
                   DefinitionType.Equals(other.DefinitionType) &&
                   Submodules.Equals(other.Submodules);
        }

        public override bool Equals(object obj)
        {
            return obj is TychoSetupModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                DefinitionKind.GetHashCode(),
                DefinitionType.GetHashCode(),
                Submodules.GetHashCode());
        }

        public static bool operator ==(TychoSetupModel left, TychoSetupModel right) => left.Equals(right);

        public static bool operator !=(TychoSetupModel left, TychoSetupModel right) => !left.Equals(right);
    }
}
