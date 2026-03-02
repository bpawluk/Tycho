using System;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models
{
    public readonly struct TychoPublisherModel : IEquatable<TychoPublisherModel>
    {
        public TypeModel DefinitionType { get; }

        public TychoDefinitionKind DefinitionKind { get; }

        public ImmutableEquatableArray<TypeModel> Events { get; }

        public TychoPublisherModel(
            TypeModel definitionType,
            TychoDefinitionKind definitionKind,
            ImmutableEquatableArray<TypeModel> events)
        {
            DefinitionType = definitionType;
            DefinitionKind = definitionKind;
            Events = events;
        }

        public bool Equals(TychoPublisherModel other)
        {
            return DefinitionType.Equals(other.DefinitionType) &&
                   DefinitionKind.Equals(other.DefinitionKind) &&
                   Events.Equals(other.Events);
        }

        public override bool Equals(object obj)
        {
            return obj is TychoPublisherModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                DefinitionType.GetHashCode(),
                DefinitionKind.GetHashCode(),
                Events.GetHashCode());
        }

        public static bool operator ==(TychoPublisherModel left, TychoPublisherModel right) => left.Equals(right);

        public static bool operator !=(TychoPublisherModel left, TychoPublisherModel right) => !left.Equals(right);
    }
}
