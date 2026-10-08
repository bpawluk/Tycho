namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class EventSerializerSymbols
    {
        public const string PayloadSerializerParameter = "payloadSerializer";

        public static string GetEventSerializerClass(string ownerClass) => $"{ownerClass}EventSerializer";
    }
}
