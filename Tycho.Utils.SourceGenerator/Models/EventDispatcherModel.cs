using System;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Models
{
    public readonly struct EventDispatcherModel : IEquatable<EventDispatcherModel>
    {
        public TypeModel DefinitionType { get; }

        public ImmutableEquatableArray<TypeModel> Events { get; }

        public EventDispatcherModel(
            TypeModel definitionType,
            ImmutableEquatableArray<TypeModel> events)
        {
            DefinitionType = definitionType;
            Events = events;
        }

        public bool Equals(EventDispatcherModel other)
        {
            return DefinitionType.Equals(other.DefinitionType) &&
                   Events.Equals(other.Events);
        }

        public override bool Equals(object obj)
        {
            return obj is EventDispatcherModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                DefinitionType.GetHashCode(),
                Events.GetHashCode());
        }

        public static bool operator ==(EventDispatcherModel left, EventDispatcherModel right) => left.Equals(right);

        public static bool operator !=(EventDispatcherModel left, EventDispatcherModel right) => !left.Equals(right);
    }
}
