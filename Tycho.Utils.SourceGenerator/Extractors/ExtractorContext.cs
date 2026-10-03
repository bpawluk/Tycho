using System.Threading;
using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Extractors
{
    internal readonly struct ExtractorContext
    {
        public Compilation Compilation { get; }

        public SemanticModelProvider SemanticModelProvider { get; }

        public CancellationToken CancellationToken { get; }

        public ExtractorContext(Compilation compilation, SemanticModelProvider semanticModelProvider, CancellationToken cancellationToken)
        {
            Compilation = compilation;
            SemanticModelProvider = semanticModelProvider;
            CancellationToken = cancellationToken;
        }
    }
}
