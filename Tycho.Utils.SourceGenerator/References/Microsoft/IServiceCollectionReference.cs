using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.Microsoft
{
    internal static class IServiceCollectionReference
    {
        private const string Namespace = "Microsoft.Extensions.DependencyInjection";
        private const string TypeName = "IServiceCollection";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);
    }
}
