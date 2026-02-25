namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class EventDispatcherSymbols
    {
        public const string DispatchMethod = "Dispatch";

        public const string EventIdParameter = "eventId";
        public const string EventPayloadParameter = "eventPayload";
        public const string EventHandlerParameter = "eventHandler";
        public const string PayloadSerializerParameter = "payloadSerializer";
        public const string CancellationTokenParameter = "cancellationToken";

        public static string GetEventDispatcherClass(string appClass) => $"{appClass}EventDispatcher";
    }
}
