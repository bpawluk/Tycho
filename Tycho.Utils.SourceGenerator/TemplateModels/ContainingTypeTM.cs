using System;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class ContainingTypeTM
    {
        public string Declaration { get; }

        public string[] Constraints { get; }

        public ContainingTypeTM(string declaration, string[] constraints)
        {
            Declaration = declaration;
            Constraints = constraints ?? Array.Empty<string>();
        }
    }
}
