using System;

namespace Tycho.Utils.SourceGenerator.Model
{
    public readonly struct EventModel : IEquatable<EventModel>
    {
        public string SourceNamespace { get; }

        public string SourceClassName { get; }

        public EventModel(
            string sourceNamespace,
            string sourceClassName)
        {
            SourceNamespace = sourceNamespace;
            SourceClassName = sourceClassName;
        }

        public readonly bool Equals(EventModel other)
        {
            return SourceNamespace == other.SourceNamespace && SourceClassName == other.SourceClassName;
        }
    }
}
