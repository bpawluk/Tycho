using System;
using Tycho.Utils.SourceGenerator.Model.Partial;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Model
{
    public readonly struct TychoDefinitionModel : IEquatable<TychoDefinitionModel>
    {
        public TypeModel DefinitionType { get; }

        public TychoDefinitionKind DefinitionKind { get; }

        public ImmutableEquatableArray<TypeModel> Submodules { get; }

        public TychoDefinitionModel(
            TypeModel definitionType, 
            TychoDefinitionKind definitionKind,
            ImmutableEquatableArray<TypeModel> submodules)
        {
            DefinitionType = definitionType;
            DefinitionKind = definitionKind;
            Submodules = submodules;
        }

        public bool Equals(TychoDefinitionModel other)
        {
            return DefinitionType.Equals(other.DefinitionType) &&
                   DefinitionKind.Equals(other.DefinitionKind) &&
                   Submodules.Equals(other.Submodules);
        }

        public override bool Equals(object obj)
        {
            return obj is TychoDefinitionModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                DefinitionType.GetHashCode(),
                DefinitionKind.GetHashCode(),
                Submodules.GetHashCode());
        }

        public static bool operator ==(TychoDefinitionModel left, TychoDefinitionModel right) => left.Equals(right);

        public static bool operator !=(TychoDefinitionModel left, TychoDefinitionModel right) => !left.Equals(right);
    }
}
