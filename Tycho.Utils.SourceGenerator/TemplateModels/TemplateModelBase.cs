using System;
using System.Collections.Generic;
using System.Linq;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class TemplateModelBase
    {
        private readonly HashSet<string> _namespaces = new HashSet<string>();

        public string Namespace { get; protected set; }

        public string[] UsedNamespaces => _namespaces
            .Where(ns => !string.Equals(ns, Namespace, StringComparison.Ordinal))
            .OrderBy(ns => ns, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        public string UseType(TypeReferenceModel typeReference)
        {
            AddUsedNamespaces(typeReference);
            return typeReference.FullReferenceName;
        }

        protected string[] UseContainingTypeDeclarations(TypeDefinitionModel typeDefinition)
        {
            if (typeDefinition.ContainingTypes.Count == 0)
            {
                return Array.Empty<string>();
            }

            foreach (TypeDefinitionModel containingType in typeDefinition.ContainingTypes)
            {
                _ = UseConstraintClauses(containingType.TypeParameters).ToArray();
            }

            return typeDefinition.ContainingTypeDeclarationSignatures.ToArray();
        }

        protected IEnumerable<string> UseConstraintClauses(IEnumerable<TypeParameterModel> typeParameters)
        {
            if (typeParameters == null)
            {
                yield break;
            }

            foreach (TypeParameterModel typeParameter in typeParameters)
            {
                foreach (TypeParameterConstraintModel constraint in typeParameter.Constraints)
                {
                    if (constraint.Type.HasValue)
                    {
                        UseType(constraint.Type.Value);
                    }
                }

                if (!string.IsNullOrEmpty(typeParameter.ConstraintsClause))
                {
                    yield return typeParameter.ConstraintsClause;
                }
            }
        }

        private void AddUsedNamespaces(TypeReferenceModel typeReference)
        {
            if (!string.IsNullOrEmpty(typeReference.Namespace))
            {
                _namespaces.Add(typeReference.Namespace);
            }

            foreach (TypeReferenceModel containingType in typeReference.ContainingTypes)
            {
                AddUsedNamespaces(containingType);
            }

            foreach (TypeArgumentModel typeArgument in typeReference.TypeArguments)
            {
                AddUsedNamespaces(typeArgument.Value);
            }
        }
    }
}
