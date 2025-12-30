namespace Tycho.Utils.SourceGenerator.Model
{
    internal struct TychoDefinitionModel
    {
        public string SourceNamespace { get; set; }

        public string SourceClassName { get; set; }

        public TychoDefinitionModel(
            string sourceNamespace,
            string sourceClassName)
        {
            SourceNamespace = sourceNamespace;
            SourceClassName = sourceClassName;
        }
    }
}
