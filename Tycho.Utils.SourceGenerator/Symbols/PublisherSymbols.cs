namespace Tycho.Utils.SourceGenerator.Symbols
{
    internal static class PublisherSymbols
    {
        public const string GenericPublisherParameter = "genericPublisher";
        public const string EventPayloadParameter = "eventPayload";
        public const string CancellationTokenParameter = "cancellationToken";

        public static string GetPublisherInterface(string ownerClass, string ownerTypeParametersSuffix = null) => $"I{ownerClass}Publisher{ownerTypeParametersSuffix ?? string.Empty}";

        public static string GetPublisherClass(string ownerClass, string ownerTypeParametersSuffix = null) => $"{ownerClass}Publisher{ownerTypeParametersSuffix ?? string.Empty}";
    }
}
