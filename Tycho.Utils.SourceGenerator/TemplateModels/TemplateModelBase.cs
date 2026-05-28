using System;
using System.Collections.Generic;
using System.Linq;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

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
            UseTypeDeep(typeReference);
            return typeReference.FullReferenceName;
        }

        protected ContainingTypeTM[] UseContainingTypes(ImmutableEquatableArray<TypeDefinitionModel> containingTypes)
        {
            ContainingTypeTM[] result = new ContainingTypeTM[containingTypes.Count];
            for (int i = 0; i < containingTypes.Count; i++)
            {
                TypeDefinitionModel containingType = containingTypes[i];
                string[] constraints = UseConstraintClauses(containingType.TypeParameters).ToArray();
                result[i] = new ContainingTypeTM(containingType.DeclarationSignature, constraints);
            }
            return result;
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

        private void UseTypeDeep(TypeReferenceModel typeReference)
        {
            if (!string.IsNullOrEmpty(typeReference.Namespace))
            {
                _namespaces.Add(typeReference.Namespace);
            }

            foreach (TypeReferenceModel containingType in typeReference.ContainingTypes)
            {
                UseTypeDeep(containingType);
            }

            foreach (TypeArgumentModel typeArgument in typeReference.TypeArguments)
            {
                UseTypeDeep(typeArgument.Value);
            }
        }
    }
}
