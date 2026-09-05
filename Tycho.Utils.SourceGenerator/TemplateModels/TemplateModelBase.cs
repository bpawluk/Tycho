using System.Collections.Generic;
using System.Linq;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class TemplateModelBase
    {
        public string Namespace { get; protected set; }

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
                if (!string.IsNullOrEmpty(typeParameter.ConstraintsClause))
                {
                    yield return typeParameter.ConstraintsClause;
                }
            }
        }

    }
}
