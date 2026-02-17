namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class EventDispatcherSymbols
    {
        public const string PayloadSerializerFieldName = "_payloadSerializer";
        public const string PayloadSerializerParameterName = "payloadSerializer";
        public const string DispatchMethodName = "Dispatch";
        public const string EventIdParameterName = "eventId";
        public const string EventPayloadParameterName = "eventPayload";
        public const string EventHandlerParameterName = "eventHandler";
        public const string CancellationTokenParameterName = "cancellationToken";
        public const string CastHandlerVariableName = "castHandler";
        public const string DispatchAsMethodName = "DispatchAs";
        public const string TEventTypeParameterName = "TEvent";
        public const string DeserializedPayloadVariableName = "deserializedPayload";
        public const string ContextVariableName = "context";

        public static string GetEventDispatcherClass(string appClass) => $"{appClass}EventDispatcher";
    }
}
