using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.Microsoft
{
    internal static class IServiceCollectionReference
    {
        private const string _namespace = "Microsoft.Extensions.DependencyInjection";
        private const string _typeName = "IServiceCollection";

        public static TypeModel TypeModel => new TypeModel(_namespace, ImmutableEquatableArray<string>.Empty, _typeName);
    }
}
