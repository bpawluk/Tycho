using System;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.Models
{
    public readonly struct TychoAppBuilderModel : IEquatable<TychoAppBuilderModel>
    {
        public TypeDefinitionModel DefinitionType { get; }

        public TychoAppBuilderModel(TypeDefinitionModel definitionType)
        {
            DefinitionType = definitionType;
        }

        public bool Equals(TychoAppBuilderModel other)
        {
            return DefinitionType.Equals(other.DefinitionType);
        }

        public override bool Equals(object obj)
        {
            return obj is TychoAppBuilderModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return DefinitionType.GetHashCode();
        }

        public static bool operator ==(TychoAppBuilderModel left, TychoAppBuilderModel right) => left.Equals(right);

        public static bool operator !=(TychoAppBuilderModel left, TychoAppBuilderModel right) => !left.Equals(right);
    }
}
