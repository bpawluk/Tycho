using System;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models
{
    public readonly struct TychoEventSerializerModel : IEquatable<TychoEventSerializerModel>
    {
        public TychoDefinitionKind DefinitionKind { get; }

        public TypeModel DefinitionType { get; }

        public ImmutableEquatableArray<TypeModel> Events { get; }

        public TychoEventSerializerModel(
            TychoDefinitionKind definitionKind,
            TypeModel definitionType,
            ImmutableEquatableArray<TypeModel> events)
        {
            DefinitionKind = definitionKind;
            DefinitionType = definitionType;
            Events = events;
        }

        public bool Equals(TychoEventSerializerModel other)
        {
            return DefinitionKind.Equals(other.DefinitionKind) &&
                   DefinitionType.Equals(other.DefinitionType) &&
                   Events.Equals(other.Events);
        }

        public override bool Equals(object obj)
        {
            return obj is TychoEventSerializerModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                DefinitionKind.GetHashCode(),
                DefinitionType.GetHashCode(),
                Events.GetHashCode());
        }

        public static bool operator ==(TychoEventSerializerModel left, TychoEventSerializerModel right) => left.Equals(right);

        public static bool operator !=(TychoEventSerializerModel left, TychoEventSerializerModel right) => !left.Equals(right);
    }
}
