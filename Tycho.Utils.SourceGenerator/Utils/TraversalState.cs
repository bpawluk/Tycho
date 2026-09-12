using System.Collections.Generic;

namespace Tycho.Utils.SourceGenerator.Utils
{
    internal sealed class TraversalState<TNode>
    {
        private readonly Stack<TNode> _pendingNodes = new Stack<TNode>();

        public void SaveToVisit(TNode node)
        {
            _pendingNodes.Push(node);
        }

        public bool GetNextToVisit(out TNode node)
        {
            if (_pendingNodes.Count > 0)
            {
                node = _pendingNodes.Pop();
                return true;
            }

            node = default;
            return false;
        }
    }
}
