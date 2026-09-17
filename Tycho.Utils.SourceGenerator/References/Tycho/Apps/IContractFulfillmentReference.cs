using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.Tycho.Apps
{
    internal static class IContractFulfillmentReference
    {
        private const string Namespace = "Tycho.Apps";
        private const string TypeName = "IContractFulfillment";

        public static TypeReferenceModel TypeModel => new TypeReferenceModel(Namespace, TypeName);
    }
}
