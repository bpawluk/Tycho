using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Apps;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class AppBuilderTM : TemplateModelBase
    {
        public string[] TypeParameters { get; }
        public string TypeParametersSuffix { get; }
        public string[] TypeParametersConstraints { get; }
        public ClassesTM Classes { get; }
        public FieldsTM Fields { get; }
        public InterfacesTM Interfaces { get; }
        public MethodsTM Methods { get; }
        public ParametersTM Parameters { get; }

        public AppBuilderTM(TychoAppBuilderModel model)
        {
            Namespace = model.DefinitionType.Namespace;
            TypeParameters = model.DefinitionType.ContainingTypes
                .SelectMany(type => type.TypeParameters.Select(parameter => parameter.Name))
                .Concat(model.DefinitionType.TypeParameters.Select(parameter => parameter.Name))
                .Distinct()
                .ToArray();
            TypeParametersSuffix = TypeParameters.Length == 0
                ? string.Empty
                : $"<{string.Join(", ", TypeParameters)}>";
            TypeParametersConstraints = UseConstraintClauses(
                    model.DefinitionType.ContainingTypes
                        .SelectMany(type => type.TypeParameters)
                        .Concat(model.DefinitionType.TypeParameters))
                .Distinct()
                .ToArray();

            Classes = new ClassesTM(model);
            Fields = new FieldsTM();
            Interfaces = new InterfacesTM(this, model);
            Methods = new MethodsTM();
            Parameters = new ParametersTM();
        }

        internal class ClassesTM
        {
            public string AppBuilderClass { get; }
            public string FacadeClass { get; }

            public ClassesTM(TychoAppBuilderModel model)
            {
                string appName = model.DefinitionType.Name;
                string appTypeSuffix = model.DefinitionType.TypeParametersSuffix;
                AppBuilderClass = AppBuilderSymbols.GetAppBuilderClass(appName);
                FacadeClass = AppFacadeSymbols.GetAppFacadeClass(appName, appTypeSuffix);
            }
        }

        internal class FieldsTM
        {
            public string AppBuilderBaseField { get; } = AppBuilderSymbols.AppBuilderBaseField;
        }

        internal class InterfacesTM
        {
            public string AppInterface { get; }
            public string AppBuilderBaseInterface { get; }
            public string FacadeInterface { get; }
            public string ServiceProviderInterface { get; }

            public InterfacesTM(AppBuilderTM owner, TychoAppBuilderModel model)
            {
                AppInterface = owner.UseType(IAppReference.TypeModel);
                AppBuilderBaseInterface = owner.UseType(IAppBuilderBaseReference.TypeModel);
                FacadeInterface = AppFacadeSymbols.GetAppFacadeInterface(
                    model.DefinitionType.Name,
                    model.DefinitionType.TypeParametersSuffix);
                ServiceProviderInterface = owner.UseType(IServiceProviderReference.TypeModel);
            }
        }

        internal class MethodsTM
        {
            public string BuildBaseMethod { get; } = IAppBuilderBaseReference.BuildMethodSignature.MethodName;
            public string BuildMethod { get; } = AppBuilderSymbols.BuildMethod;
        }

        internal class ParametersTM
        {
            public string AppParameter { get; } = AppBuilderSymbols.AppParameter;
            public string AppBuilderBaseParameter { get; } = AppBuilderSymbols.AppBuilderBaseParameter;
            public string ParentServiceProviderParameter { get; } = AppBuilderSymbols.ParentServiceProviderParameter;
        }
    }
}
