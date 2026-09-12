using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.Microsoft;
using Tycho.Utils.SourceGenerator.References.Tycho.Events;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class ModuleSetupTM : TemplateModelBase
    {
        public ContainingTypeTM[] ContainingTypes { get; }

        public string[] OwnerConstraints { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public SubmoduleTM[] Submodules { get; }

        public ModuleSetupTM(TychoSetupModel tychoDefinitionModel)
        {
            Namespace = tychoDefinitionModel.DefinitionType.Namespace;
            ContainingTypes = UseContainingTypes(tychoDefinitionModel.DefinitionType.ContainingTypes);
            OwnerConstraints = UseConstraintClauses(tychoDefinitionModel.DefinitionType.TypeParameters).ToArray();
            Classes = new ClassesTM(this, tychoDefinitionModel);
            Interfaces = new InterfacesTM(this, tychoDefinitionModel);
            Methods = new MethodsTM();
            Parameters = new ParametersTM();
            Submodules = tychoDefinitionModel.Submodules.Select(s => new SubmoduleTM(this, s)).ToArray();
        }

        internal class ClassesTM
        {
            public string SetupClass { get; }
            public string ModuleParentClass { get; }
            public string PublisherClass { get; }
            public string EventSerializerClass { get; }
            public string ServiceCollectionServiceExtensionsClass { get; }

            public ClassesTM(ModuleSetupTM owner, TychoSetupModel tychoDefinitionModel)
            {
                string moduleNameStem = tychoDefinitionModel.DefinitionType.Name;
                var setupType = new GeneratedTypeModel(
                    tychoDefinitionModel.DefinitionType,
                    ModuleSetupSymbols.GetSetupClass(moduleNameStem));
                var parentType = new GeneratedTypeModel(
                    tychoDefinitionModel.DefinitionType,
                    ModuleParentSymbols.GetParentClass(moduleNameStem));
                var publisherType = new GeneratedTypeModel(
                    tychoDefinitionModel.DefinitionType,
                    PublisherSymbols.GetPublisherClass(moduleNameStem));
                var eventSerializerType = new GeneratedTypeModel(
                    tychoDefinitionModel.DefinitionType,
                    EventSerializerSymbols.GetEventSerializerClass(moduleNameStem));

                SetupClass = setupType.DeclarationName;
                ModuleParentClass = parentType.ReferenceName;
                PublisherClass = publisherType.ReferenceName;
                EventSerializerClass = eventSerializerType.ReferenceName;
                ServiceCollectionServiceExtensionsClass = ServiceCollectionServiceExtensionsReference.TypeModel.FullReferenceName;
            }
        }

        internal class InterfacesTM
        {
            public string ModuleParentInterface { get; }
            public string PublisherInterface { get; }
            public string EventSerializerInterface { get; }
            public string ServiceCollectionInterface { get; }

            public InterfacesTM(ModuleSetupTM owner, TychoSetupModel tychoDefinitionModel)
            {
                var parentInterfaceType = new GeneratedTypeModel(
                    tychoDefinitionModel.DefinitionType,
                    ModuleParentSymbols.GetParentInterface(tychoDefinitionModel.DefinitionType.Name));
                var publisherInterfaceType = new GeneratedTypeModel(
                    tychoDefinitionModel.DefinitionType,
                    PublisherSymbols.GetPublisherInterface(tychoDefinitionModel.DefinitionType.Name));
                ModuleParentInterface = parentInterfaceType.ReferenceName;
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
                SetupMethod = ModuleSetupSymbols.SetupMethod;
                AddSingletonMethod = ServiceCollectionServiceExtensionsReference.AddSingletonMethodName;
                AddTransientMethod = ServiceCollectionServiceExtensionsReference.AddTransientMethodName;
            }
        }

        internal class ParametersTM
        {
            public string ModuleParameter { get; }

            public ParametersTM()
            {
                ModuleParameter = ModuleSetupSymbols.ModuleParameter;
            }
        }

        internal class SubmoduleTM
        {
            public string FacadeInterface { get; }
            public string FacadeClass { get; }

            public SubmoduleTM(ModuleSetupTM owner, TypeReferenceModel moduleType)
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
