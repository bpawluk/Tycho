using System;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models
{
    public readonly struct TychoFacadeModel : IEquatable<TychoFacadeModel>
    {
        public TychoDefinitionKind DefinitionKind { get; }

        public TypeDefinitionModel DefinitionType { get; }

        public ImmutableEquatableArray<TychoRequestModel> Requests { get; }

        public TychoFacadeModel(
            TychoDefinitionKind definitionKind,
            TypeDefinitionModel definitionType,
            ImmutableEquatableArray<TychoRequestModel> requests)
        {
            DefinitionKind = definitionKind;
            DefinitionType = definitionType;
            Requests = requests;
        }

        public bool Equals(TychoFacadeModel other)
        {
            return DefinitionKind.Equals(other.DefinitionKind) &&
                   DefinitionType.Equals(other.DefinitionType) &&
                   Requests.Equals(other.Requests);
        }

        public override bool Equals(object obj)
        {
            return obj is TychoFacadeModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                DefinitionKind.GetHashCode(),
                DefinitionType.GetHashCode(),
                Requests.GetHashCode());
        }

        public static bool operator ==(TychoFacadeModel left, TychoFacadeModel right) => left.Equals(right);

        public static bool operator !=(TychoFacadeModel left, TychoFacadeModel right) => !left.Equals(right);
    }
}
