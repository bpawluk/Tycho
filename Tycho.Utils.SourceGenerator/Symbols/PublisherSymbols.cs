namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class PublisherSymbols
    {
        public const string PublisherInterface = "IPublisher";
        public const string GenericPublisherParameter = "genericPublisher";
        public const string EventPayloadParameter = "eventPayload";
        public const string CancellationTokenParameter = "cancellationToken";

        public static string GetPublisherClass(string ownerClass) => $"{ownerClass}Publisher";
    }
}
