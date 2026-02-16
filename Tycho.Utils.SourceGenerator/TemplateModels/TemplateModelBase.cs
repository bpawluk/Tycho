using System;
using System.Collections.Generic;
using System.Linq;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class TemplateModelBase
    {
        private readonly HashSet<string> _namespaces = new HashSet<string>();

        public string[] UsedNamespaces => _namespaces.OrderBy(ns => ns, StringComparer.OrdinalIgnoreCase).ToArray();

        public string UseType(TypeModel typeModel)
        {
            if (!string.IsNullOrEmpty(typeModel.Namespace))
            {
                _namespaces.Add(typeModel.Namespace);
            }
            return typeModel.PathName;
        }
    }
}
