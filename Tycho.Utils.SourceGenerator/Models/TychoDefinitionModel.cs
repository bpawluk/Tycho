using System;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models
{
    public readonly struct TychoDefinitionModel : IEquatable<TychoDefinitionModel>
    {
        public TychoDefinitionKind DefinitionKind { get; }

        public TypeModel DefinitionType { get; }

        public ImmutableEquatableArray<TypeModel> Submodules { get; }

        public TychoDefinitionModel(
            TychoDefinitionKind definitionKind,
            TypeModel definitionType,
            ImmutableEquatableArray<TypeModel> submodules)
        {
            DefinitionKind = definitionKind;
            DefinitionType = definitionType;
            Submodules = submodules;
        }

        public bool Equals(TychoDefinitionModel other)
        {
            return DefinitionKind.Equals(other.DefinitionKind) &&
                   DefinitionType.Equals(other.DefinitionType) &&
                   Submodules.Equals(other.Submodules);
        }

        public override bool Equals(object obj)
        {
            return obj is TychoDefinitionModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                DefinitionKind.GetHashCode(),
                DefinitionType.GetHashCode(),
                Submodules.GetHashCode());
        }

        public static bool operator ==(TychoDefinitionModel left, TychoDefinitionModel right) => left.Equals(right);

        public static bool operator !=(TychoDefinitionModel left, TychoDefinitionModel right) => !left.Equals(right);
    }
}
