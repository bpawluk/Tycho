namespace Tycho.Utils.SourceGenerator.Model
{
    public readonly struct TychoDefinitionModel
    {
        public string SourceNamespace { get; }

        public string SourceClassName { get; }

        public ImmutableEquatableArray<EventModel> Events { get; }

        public TychoDefinitionModel(
            string sourceNamespace,
            string sourceClassName,
            ImmutableEquatableArray<EventModel> events)
        {
            SourceNamespace = sourceNamespace;
            SourceClassName = sourceClassName;
            Events = events;
        }
    }
}
