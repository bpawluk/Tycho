using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Apps;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class AppBuilderTM : TemplateModelBase
    {
        public ContainingTypeTM[] ContainingTypes { get; }
        public string[] OwnerConstraints { get; }
        public ClassesTM Classes { get; }
        public FieldsTM Fields { get; }
        public InterfacesTM Interfaces { get; }
        public MethodsTM Methods { get; }
        public ParametersTM Parameters { get; }

        public AppBuilderTM(TychoAppBuilderModel model)
        {
            Namespace = model.DefinitionType.Namespace;
            ContainingTypes = UseContainingTypes(model.DefinitionType.ContainingTypes);
            OwnerConstraints = UseConstraintClauses(model.DefinitionType.TypeParameters).ToArray();
            Classes = new ClassesTM(model);
            Fields = new FieldsTM();
            Interfaces = new InterfacesTM(this, model);
            Methods = new MethodsTM();
            Parameters = new ParametersTM();
        }

        internal class ClassesTM
        {
            public string AppBuilderClass { get; }
            public string AppBuilderDeclaration { get; }
            public string FacadeClass { get; }

            public ClassesTM(TychoAppBuilderModel model)
            {
                var builderType = new GeneratedTypeModel(
                    model.DefinitionType,
                    AppBuilderSymbols.GetAppBuilderClass(model.DefinitionType.Name));
                var facadeType = new GeneratedTypeModel(
                    model.DefinitionType,
                    AppFacadeSymbols.GetAppFacadeClass(model.DefinitionType.Name));
                AppBuilderClass = builderType.Identifier;
                AppBuilderDeclaration = builderType.DeclarationName;
                FacadeClass = facadeType.ReferenceName;
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
                var facadeInterfaceType = new GeneratedTypeModel(
                    model.DefinitionType,
                    AppFacadeSymbols.GetAppFacadeInterface(model.DefinitionType.Name));
                AppInterface = owner.UseType(IAppReference.TypeModel);
                AppBuilderBaseInterface = owner.UseType(IAppBuilderBaseReference.TypeModel);
                FacadeInterface = facadeInterfaceType.ReferenceName;
                ServiceProviderInterface = owner.UseType(IServiceProviderReference.TypeModel);
            }
        }

        internal class MethodsTM
        {
            public string BuildBaseMethod { get; } = IAppBuilderBaseReference.BuildMethodName;
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
