using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Scriban;
using Tycho.Utils.SourceGenerator.Model;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator
{
    public abstract class TychoSourceGeneratorBase : IIncrementalGenerator
    {
        protected abstract string AttributeName { get; }

        protected abstract string EventsDefinitionMethodName { get; }

        protected abstract string EventsDefinitionTypeName { get; }

        protected abstract string EventHandlerDefinitionMethodName { get; }

        public abstract void Initialize(IncrementalGeneratorInitializationContext context);

        protected IncrementalValuesProvider<TychoDefinitionModel> BuildPipeline(IncrementalGeneratorInitializationContext context)
        {
            return context.SyntaxProvider.ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: AttributeName,
                predicate: GeneratorPredicate,
                transform: BuildGeneratorModel
            );
        }

        protected void GenerateSource(
            SourceProductionContext context,
            TychoDefinitionModel model,
            string templatePath,
            string targetFileName)
        {
            var template = Template.Parse(EmbeddedResource.GetContent(templatePath), templatePath);
            var output = template.Render(model);
            var sourceText = SourceText.From(output, Encoding.UTF8);
            context.AddSource(targetFileName, sourceText);
        }

        private bool GeneratorPredicate(SyntaxNode _, CancellationToken __)
        {
            return true;
        }

        private TychoDefinitionModel BuildGeneratorModel(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
        {
            var appClass = context.TargetSymbol;
            var appNamespace = appClass
                .ContainingNamespace
                .ToDisplayString(SymbolDisplayFormat
                    .FullyQualifiedFormat
                    .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted));

            var events = ExtractEvents(context, cancellationToken);

            return new TychoDefinitionModel(appNamespace, appClass.Name, events);
        }

        private ImmutableEquatableArray<EventModel> ExtractEvents(
            GeneratorAttributeSyntaxContext context,
            CancellationToken cancellationToken)
        {
            if (!(context.TargetSymbol is INamedTypeSymbol tychoDefinitionTypeSymbol))
            {
                return ImmutableEquatableArray<EventModel>.Empty;
            }

            var events = new HashSet<EventModel>();

            foreach (var declaringSyntax in tychoDefinitionTypeSymbol.DeclaringSyntaxReferences)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!(declaringSyntax.GetSyntax(cancellationToken) is TypeDeclarationSyntax typeDeclaration))
                {
                    continue;
                }

                var tychoDefinitionTypeSemanticModel = context.SemanticModel.Compilation.GetSemanticModel(typeDeclaration.SyntaxTree);

                foreach (var member in typeDeclaration.Members)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!(member is MethodDeclarationSyntax methodSyntax))
                    {
                        continue;
                    }

                    if (methodSyntax.Body == null)
                    {
                        continue;
                    }

                    if (!string.Equals(methodSyntax.Identifier.ValueText, EventsDefinitionMethodName, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    if (!(tychoDefinitionTypeSemanticModel.GetDeclaredSymbol(methodSyntax, cancellationToken) is IMethodSymbol methodSymbol))
                    {
                        continue;
                    }

                    if (methodSymbol.Parameters.Length != 1 || !IsEventsDefinitionType(methodSymbol.Parameters[0].Type))
                    {
                        continue;
                    }

                    foreach (var invocation in methodSyntax.Body.DescendantNodes().OfType<InvocationExpressionSyntax>())
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (!(tychoDefinitionTypeSemanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol is IMethodSymbol invokedMethodSymbol))
                        {
                            continue;
                        }

                        if (!IsEventsDefinitionType(invokedMethodSymbol.ReceiverType))
                        {
                            continue;
                        }

                        if (!string.Equals(invokedMethodSymbol.Name, EventHandlerDefinitionMethodName, StringComparison.Ordinal))
                        {
                            continue;
                        }

                        if (invokedMethodSymbol.TypeArguments.Length < 1)
                        {
                            continue;
                        }

                        var eventType = invokedMethodSymbol.TypeArguments[0];
                        if (eventType == null)
                        {
                            continue;
                        }

                        var eventNamespace = eventType
                            .ContainingNamespace
                            .ToDisplayString(SymbolDisplayFormat
                                .FullyQualifiedFormat
                                .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted));

                        events.Add(new EventModel(eventNamespace, eventType.Name));
                    }
                }
            }

            return events.ToImmutableEquatableArray();
        }

        private bool IsEventsDefinitionType(ITypeSymbol type)
        {
            if (type == null)
            {
                return false;
            }
            return type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == EventsDefinitionTypeName;
        }
    }
}
