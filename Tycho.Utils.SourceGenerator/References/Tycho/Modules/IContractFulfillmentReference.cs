using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Modules
{
    internal static class IContractFulfillmentReference
    {
        private const string Namespace = "Tycho.Modules";
        private const string TypeName = "IContractFulfillment";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);
    }
}
