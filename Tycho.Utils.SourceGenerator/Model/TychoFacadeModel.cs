using System;
using Tycho.Utils.SourceGenerator.Model.Partial;

namespace Tycho.Utils.SourceGenerator.Model
{
    public readonly struct TychoFacadeModel : IEquatable<TychoFacadeModel>
    {
        public TypeModel DefinitionType { get; }

        public TychoFacadeModel(
            TypeModel definitionType)
        {
            DefinitionType = definitionType;
        }

        public bool Equals(TychoFacadeModel other)
        {
            return DefinitionType.Equals(other.DefinitionType);
        }

        public override bool Equals(object obj)
        {
            return obj is TychoFacadeModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return DefinitionType.GetHashCode();
        }

        public static bool operator ==(TychoFacadeModel left, TychoFacadeModel right) => left.Equals(right);

        public static bool operator !=(TychoFacadeModel left, TychoFacadeModel right) => !left.Equals(right);
    }
}
