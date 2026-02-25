namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class EventDispatcherSymbols
    {
        public const string DispatchMethodName = "Dispatch";

        public const string EventIdParameterName = "eventId";
        public const string EventPayloadParameterName = "eventPayload";
        public const string EventHandlerParameterName = "eventHandler";
        public const string PayloadSerializerParameterName = "payloadSerializer";
        public const string CancellationTokenParameterName = "cancellationToken";

        public static string GetEventDispatcherClass(string appClass) => $"{appClass}EventDispatcher";
    }
}
