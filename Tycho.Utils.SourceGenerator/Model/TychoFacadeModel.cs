using System;
using Tycho.Utils.SourceGenerator.Model.Partial;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Model
{
    public readonly struct TychoFacadeModel : IEquatable<TychoFacadeModel>
    {
        public TypeModel DefinitionType { get; }

        public TychoDefinitionKind DefinitionKind { get; }

        public ImmutableEquatableArray<TychoRequestModel> Requests { get; }

        public TychoFacadeModel(
            TypeModel definitionType,
            TychoDefinitionKind definitionKind,
            ImmutableEquatableArray<TychoRequestModel> requests)
        {
            DefinitionType = definitionType;
            DefinitionKind = definitionKind;
            Requests = requests;
        }

        public bool Equals(TychoFacadeModel other)
        {
            return DefinitionType.Equals(other.DefinitionType) &&
                   DefinitionKind.Equals(other.DefinitionKind) &&
                   Requests.Equals(other.Requests);
        }

        public override bool Equals(object obj)
        {
            return obj is TychoFacadeModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                DefinitionType.GetHashCode(),
                DefinitionKind.GetHashCode(),
                Requests.GetHashCode());
        }

        public static bool operator ==(TychoFacadeModel left, TychoFacadeModel right) => left.Equals(right);

        public static bool operator !=(TychoFacadeModel left, TychoFacadeModel right) => !left.Equals(right);
    }
}
