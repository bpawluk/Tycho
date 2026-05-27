using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Tycho.Utils.SourceGenerator.Extensions;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.Extractors
{
    internal static class MethodInvokationsExtractor
    {
        public static ImmutableEquatableArray<MethodInvocationModel> Extract(IMethodSymbol methodSymbol, ExtractorContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var methodInvocations = new HashSet<MethodInvocationModel>();
            var visitTracker = new VisitTracker<IMethodSymbol>(SymbolEqualityComparer.Default);

            var traversalState = new TraversalState<IMethodSymbol>();
            traversalState.SaveToVisit(methodSymbol.GetOriginalDefinition());

            while (traversalState.GetNextToVisit(out IMethodSymbol currentMethodSymbol))
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                if (!visitTracker.TryVisit(currentMethodSymbol))
                {
                    continue;
                }

                foreach (SyntaxReference syntaxReference in currentMethodSymbol.DeclaringSyntaxReferences)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();

                    SyntaxNode declarationSyntax = syntaxReference.GetSyntax(context.CancellationToken);
                    SyntaxTree declarationSyntaxTree = declarationSyntax.SyntaxTree;

                    if (!context.Compilation.ContainsSyntaxTree(declarationSyntaxTree))
                    {
                        continue;
                    }

                    if (!TryGetInvocationExpressions(declarationSyntax, out IEnumerable<InvocationExpressionSyntax> invocationExpressions))
                    {
                        continue;
                    }

                    SemanticModel semanticModel = context.SemanticModelProvider.GetSemanticModel(declarationSyntaxTree);
                    foreach (InvocationExpressionSyntax invocationSyntax in invocationExpressions)
                    {
                        context.CancellationToken.ThrowIfCancellationRequested();

                        if (TryGetInvokedMethodSymbol(semanticModel, invocationSyntax, context.CancellationToken, out IMethodSymbol invokedMethodSymbol))
                        {
                            IMethodSymbol originalInvokedMethodSymbol = invokedMethodSymbol.GetOriginalDefinition();
                            traversalState.SaveToVisit(originalInvokedMethodSymbol);
                            methodInvocations.Add(MethodInvokationExtractor.Extract(invokedMethodSymbol, context));
                        }
                    }
                }
            }

            return methodInvocations.ToImmutableEquatableArray();
        }

        private static bool TryGetInvocationExpressions(SyntaxNode declarationSyntax, out IEnumerable<InvocationExpressionSyntax> invocationExpressions)
        {
            if (declarationSyntax is MethodDeclarationSyntax methodSyntax)
            {
                invocationExpressions = GetInvocationExpressions(methodSyntax.Body, methodSyntax.ExpressionBody);
                return true;
            }

            if (declarationSyntax is LocalFunctionStatementSyntax localFunctionSyntax)
            {
                invocationExpressions = GetInvocationExpressions(localFunctionSyntax.Body, localFunctionSyntax.ExpressionBody);
                return true;
            }

            invocationExpressions = Enumerable.Empty<InvocationExpressionSyntax>();
            return false;
        }

        private static IEnumerable<InvocationExpressionSyntax> GetInvocationExpressions(BlockSyntax body, ArrowExpressionClauseSyntax expressionBody)
        {
            IEnumerable<InvocationExpressionSyntax> blockInvocations = body?
                .DescendantNodes().OfType<InvocationExpressionSyntax>()
                ?? Enumerable.Empty<InvocationExpressionSyntax>();
            IEnumerable<InvocationExpressionSyntax> expressionBodyInvocations = expressionBody?
                .DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>()
                ?? Enumerable.Empty<InvocationExpressionSyntax>();
            return blockInvocations.Concat(expressionBodyInvocations);
        }

        private static bool TryGetInvokedMethodSymbol(
            SemanticModel semanticModel,
            InvocationExpressionSyntax invocationSyntax,
            CancellationToken token,
            out IMethodSymbol invokedMethodSymbol)
        {
            invokedMethodSymbol = (semanticModel.GetOperation(invocationSyntax, token) as IInvocationOperation)?.TargetMethod;
            if (invokedMethodSymbol != null)
            {
                return true;
            }

            SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(invocationSyntax, token);
            if (symbolInfo.Symbol is IMethodSymbol resolvedMethodSymbol)
            {
                invokedMethodSymbol = resolvedMethodSymbol;
                return true;
            }

            if (symbolInfo.CandidateSymbols.Length == 1 && symbolInfo.CandidateSymbols[0] is IMethodSymbol candidateMethodSymbol)
            {
                invokedMethodSymbol = candidateMethodSymbol;
                return true;
            }

            return false;
        }
    }
}
