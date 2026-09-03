using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.Microsoft;
using Tycho.Utils.SourceGenerator.References.Tycho.Events;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class AppSetupTM : TemplateModelBase
    {
        public ContainingTypeTM[] ContainingTypes { get; }

        public string[] OwnerConstraints { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public SubmoduleTM[] Submodules { get; }

        public AppSetupTM(TychoSetupModel tychoSetupModel)
        {
            Namespace = tychoSetupModel.DefinitionType.Namespace;
            ContainingTypes = UseContainingTypes(tychoSetupModel.DefinitionType.ContainingTypes);
            OwnerConstraints = UseConstraintClauses(tychoSetupModel.DefinitionType.TypeParameters).ToArray();
            Classes = new ClassesTM(this, tychoSetupModel);
            Interfaces = new InterfacesTM(this, tychoSetupModel);
            Methods = new MethodsTM();
            Parameters = new ParametersTM();
            Submodules = tychoSetupModel.Submodules.Select(s => new SubmoduleTM(this, s)).ToArray();
        }

        internal class ClassesTM
        {
            public string SetupClass { get; }
            public string PublisherClass { get; }
            public string EventSerializerClass { get; }
            public string ServiceCollectionServiceExtensionsClass { get; }

            public ClassesTM(AppSetupTM owner, TychoSetupModel tychoSetupModel)
            {
                string appNameStem = tychoSetupModel.DefinitionType.Name;
                var setupType = new GeneratedTypeModel(
                    tychoSetupModel.DefinitionType,
                    AppSetupSymbols.GetSetupClass(appNameStem));
                var publisherType = new GeneratedTypeModel(
                    tychoSetupModel.DefinitionType,
                    PublisherSymbols.GetPublisherClass(appNameStem));
                var eventSerializerType = new GeneratedTypeModel(
                    tychoSetupModel.DefinitionType,
                    EventSerializerSymbols.GetEventSerializerClass(appNameStem));

                SetupClass = setupType.DeclarationName;
                PublisherClass = publisherType.ReferenceName;
                EventSerializerClass = eventSerializerType.ReferenceName;
                ServiceCollectionServiceExtensionsClass = ServiceCollectionServiceExtensionsReference.TypeModel.FullReferenceName;
            }
        }

        internal class InterfacesTM
        {
            public string PublisherInterface { get; }
            public string EventSerializerInterface { get; }
            public string ServiceCollectionInterface { get; }

            public InterfacesTM(AppSetupTM owner, TychoSetupModel tychoSetupModel)
            {
                var publisherInterfaceType = new GeneratedTypeModel(
                    tychoSetupModel.DefinitionType,
                    PublisherSymbols.GetPublisherInterface(tychoSetupModel.DefinitionType.Name));
                PublisherInterface = publisherInterfaceType.ReferenceName;
                EventSerializerInterface = IEventSerializerReference.TypeModel.FullReferenceName;
                ServiceCollectionInterface = IServiceCollectionReference.TypeModel.FullReferenceName;
            }
        }

        internal class MethodsTM
        {
            public string SetupMethod { get; }
            public string AddSingletonMethod { get; }
            public string AddTransientMethod { get; }

            public MethodsTM()
            {
                SetupMethod = AppSetupSymbols.SetupMethod;
                AddSingletonMethod = ServiceCollectionServiceExtensionsReference.AddSingletonMethodName;
                AddTransientMethod = ServiceCollectionServiceExtensionsReference.AddTransientMethodName;
            }
        }

        internal class ParametersTM
        {
            public string AppParameter { get; }

            public ParametersTM()
            {
                AppParameter = AppSetupSymbols.AppParameter;
            }
        }

        internal class SubmoduleTM
        {
            public string FacadeInterface { get; }
            public string FacadeClass { get; }

            public SubmoduleTM(AppSetupTM owner, TypeReferenceModel moduleType)
            {
                var facadeInterfaceType = new GeneratedTypeModel(
                    moduleType,
                    ModuleFacadeSymbols.GetModuleFacadeInterface(moduleType.Name));
                var facadeType = new GeneratedTypeModel(
                    moduleType,
                    ModuleFacadeSymbols.GetModuleFacadeClass(moduleType.Name));
                FacadeInterface = facadeInterfaceType.TypeReference.FullReferenceName;
                FacadeClass = facadeType.TypeReference.FullReferenceName;
            }
        }
    }
}
