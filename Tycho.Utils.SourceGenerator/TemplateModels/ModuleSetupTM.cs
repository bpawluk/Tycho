using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.Microsoft;
using Tycho.Utils.SourceGenerator.References.Tycho.Events;
using Tycho.Utils.SourceGenerator.References.Tycho.Modules;
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
            public string ModuleClass { get; }
            public string ModuleParentClass { get; }
            public string PublisherClass { get; }
            public string EventSerializerClass { get; }
            public string BaseClass { get; }
            public string ServiceCollectionServiceExtensionsClass { get; }
            public string ServiceProviderServiceExtensionsClass { get; }

            public ClassesTM(ModuleSetupTM owner, TychoSetupModel tychoDefinitionModel)
            {
                string moduleNameStem = tychoDefinitionModel.DefinitionType.Name;
                string moduleTypeSuffix = tychoDefinitionModel.DefinitionType.TypeParametersSuffix;

                ModuleClass = tychoDefinitionModel.DefinitionType.DeclarationName;
                ModuleParentClass = ModuleParentSymbols.GetParentClass(moduleNameStem, moduleTypeSuffix);
                PublisherClass = PublisherSymbols.GetPublisherClass(moduleNameStem, moduleTypeSuffix);
                EventSerializerClass = EventSerializerSymbols.GetEventSerializerClass(moduleNameStem, moduleTypeSuffix);
                BaseClass = owner.UseType(TychoModuleReference.TypeModel);
                ServiceCollectionServiceExtensionsClass = owner.UseType(ServiceCollectionServiceExtensionsReference.TypeModel);
                ServiceProviderServiceExtensionsClass = owner.UseType(ServiceProviderServiceExtensionsReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string ModuleParentInterface { get; }
            public string PublisherInterface { get; }
            public string EventSerializerInterface { get; }
            public string ServiceCollectionInterface { get; }
            public string ModuleInstanceInterface { get; }

            public InterfacesTM(ModuleSetupTM owner, TychoSetupModel tychoDefinitionModel)
            {
                ModuleParentInterface = ModuleParentSymbols.GetParentInterface(tychoDefinitionModel.DefinitionType.Name, tychoDefinitionModel.DefinitionType.TypeParametersSuffix);
                PublisherInterface = PublisherSymbols.GetPublisherInterface(tychoDefinitionModel.DefinitionType.Name, tychoDefinitionModel.DefinitionType.TypeParametersSuffix);
                EventSerializerInterface = owner.UseType(IEventSerializerReference.TypeModel);
                ServiceCollectionInterface = owner.UseType(IServiceCollectionReference.TypeModel);
                ModuleInstanceInterface = owner.UseType(IModuleReference.TypeModel);
            }
        }

        internal class MethodsTM
        {
            public string AutoSetupMethod { get; }
            public string AddSingletonMethod { get; }
            public string AddTransientMethod { get; }
            public string GetRequiredServiceMethod { get; }

            public MethodsTM()
            {
                AutoSetupMethod = TychoModuleReference.AutoSetupMethodSignature.MethodName;
                AddSingletonMethod = ServiceCollectionServiceExtensionsReference.AddSingletonMethodSignature.MethodName;
                AddTransientMethod = ServiceCollectionServiceExtensionsReference.AddTransientMethodSignature.MethodName;
                GetRequiredServiceMethod = ServiceProviderServiceExtensionsReference.GetRequiredServiceMethodSignature.MethodName;
            }
        }

        internal class ParametersTM
        {
            public string ModuleParameter { get; }
            public string ProviderParameter { get; }

            public ParametersTM()
            {
                ModuleParameter = ModuleDefinitionSymbols.ModuleParameter;
                ProviderParameter = ModuleDefinitionSymbols.ProviderParameter;
            }
        }

        internal class SubmoduleTM
        {
            public string ModuleClass { get; }
            public string FacadeInterface { get; }
            public string FacadeClass { get; }

            public SubmoduleTM(ModuleSetupTM owner, TypeReferenceModel moduleType)
            {
                ModuleClass = owner.UseType(moduleType);
                string moduleNameStem = moduleType.Name;
                string moduleTypeSuffix = moduleType.TypeArgumentsSuffix;
                FacadeInterface = ModuleFacadeSymbols.GetModuleFacadeInterface(moduleNameStem, moduleTypeSuffix);
                FacadeClass = ModuleFacadeSymbols.GetModuleFacadeClass(moduleNameStem, moduleTypeSuffix);
            }
        }
    }
}
