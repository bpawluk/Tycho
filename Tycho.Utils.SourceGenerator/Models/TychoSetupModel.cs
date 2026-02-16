using System;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.Models
{
    public readonly struct TychoSetupModel : IEquatable<TychoSetupModel>
    {
        public TypeModel DefinitionType { get; }

        public TychoSetupModel(
            TypeModel definitionType)
        {
            DefinitionType = definitionType;
        }

        public bool Equals(TychoSetupModel other)
        {
            return DefinitionType.Equals(other.DefinitionType);
        }

        public override bool Equals(object obj)
        {
            return obj is TychoSetupModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return DefinitionType.GetHashCode();
        }

        public static bool operator ==(TychoSetupModel left, TychoSetupModel right) => left.Equals(right);

        public static bool operator !=(TychoSetupModel left, TychoSetupModel right) => !left.Equals(right);
    }
}
