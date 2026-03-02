using System;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models
{
    public readonly struct TychoParentModel : IEquatable<TychoParentModel>
    {
        public TypeModel DefinitionType { get; }

        public ImmutableEquatableArray<TychoRequestModel> Requests { get; }

        public TychoParentModel(
            TypeModel definitionType,
            ImmutableEquatableArray<TychoRequestModel> requests)
        {
            DefinitionType = definitionType;
            Requests = requests;
        }

        public bool Equals(TychoParentModel other)
        {
            return DefinitionType.Equals(other.DefinitionType) &&
                   Requests.Equals(other.Requests);
        }

        public override bool Equals(object obj)
        {
            return obj is TychoParentModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                DefinitionType.GetHashCode(),
                Requests.GetHashCode());
        }

        public static bool operator ==(TychoParentModel left, TychoParentModel right) => left.Equals(right);

        public static bool operator !=(TychoParentModel left, TychoParentModel right) => !left.Equals(right);
    }
}
