namespace Tycho.Utils.SourceGenerator.Models.System
{
    internal readonly struct GeneratedTypeModel
    {
        public string Identifier { get; }

        public TypeReferenceModel TypeReference { get; }

        public string DeclarationName => TypeReference.ReferenceName;

        public string ReferenceName => TypeReference.ReferenceName;

        public string FullReferenceName => TypeReference.FullReferenceName;

        public GeneratedTypeModel(TypeDefinitionModel ownerType, string identifier) : this(ownerType.GetReference(), identifier)
        {
        }

        public GeneratedTypeModel(TypeReferenceModel ownerType, string identifier)
        {
            Identifier = identifier ?? string.Empty;
            TypeReference = new TypeReferenceModel(
                ownerType.Namespace,
                ownerType.ContainingTypes,
                Identifier,
                ownerType.TypeArguments);
        }
    }
}
