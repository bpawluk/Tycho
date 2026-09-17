using System;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.Models
{
    public readonly struct TychoExtensionsModel : IEquatable<TychoExtensionsModel>
    {
        public TypeDefinitionModel DefinitionType { get; }

        public TychoExtensionsModel(
            TypeDefinitionModel definitionType)
        {
            DefinitionType = definitionType;
        }

        public bool Equals(TychoExtensionsModel other)
        {
            return DefinitionType.Equals(other.DefinitionType);
        }

        public override bool Equals(object obj)
        {
            return obj is TychoExtensionsModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return DefinitionType.GetHashCode();
        }

        public static bool operator ==(TychoExtensionsModel left, TychoExtensionsModel right) => left.Equals(right);

        public static bool operator !=(TychoExtensionsModel left, TychoExtensionsModel right) => !left.Equals(right);
    }
}
