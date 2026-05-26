using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Tycho.Utils.SourceGenerator.Utils
{
    internal sealed class VisitTracker<TNode>
    {
        private readonly HashSet<TNode> _visitedNodes;

        public VisitTracker(IEqualityComparer<TNode> comparer)
        {
            _visitedNodes = new HashSet<TNode>(comparer);
        }

        public bool TryVisit(TNode node)
        {
            return _visitedNodes.Add(node);
        }
    }
}
