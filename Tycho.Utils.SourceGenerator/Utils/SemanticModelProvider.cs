using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Tycho.Utils.SourceGenerator.Utils
{
    internal sealed class SemanticModelProvider
    {
        private readonly Compilation _compilation;

        private readonly Dictionary<SyntaxTree, SemanticModel> _semanticModels = new Dictionary<SyntaxTree, SemanticModel>();

        public SemanticModelProvider(Compilation compilation)
        {
            _compilation = compilation;
        }

        public SemanticModel GetSemanticModel(SyntaxTree syntaxTree)
        {
            if (!_semanticModels.TryGetValue(syntaxTree, out SemanticModel semanticModel))
            {
                semanticModel = _compilation.GetSemanticModel(syntaxTree);
                _semanticModels[syntaxTree] = semanticModel;
            }
            return semanticModel;
        }
    }
}
