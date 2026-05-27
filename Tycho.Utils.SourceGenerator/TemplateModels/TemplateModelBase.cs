using System;
using System.Collections.Generic;
using System.Linq;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class TemplateModelBase
    {
        private readonly HashSet<string> _namespaces = new HashSet<string>();

        public string Namespace { get; protected set; }

        public string[] UsedNamespaces => _namespaces
            .Where(ns => ns != Namespace)
            .OrderBy(ns => ns, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        public string UseType(TypeModel typeModel)
        {
            if (!string.IsNullOrEmpty(typeModel.Namespace))
            {
                _namespaces.Add(typeModel.Namespace);
            }
            return typeModel.FullReferenceName;
        }

        public string UseType(TypeReferenceModel typeReference)
        {
            if (!string.IsNullOrEmpty(typeReference.Namespace))
            {
                _namespaces.Add(typeReference.Namespace);
            }
            return typeReference.Name;
        }
    }
}
