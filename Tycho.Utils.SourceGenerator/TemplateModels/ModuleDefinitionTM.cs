using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.Microsoft;
using Tycho.Utils.SourceGenerator.References.Tycho.Events;
using Tycho.Utils.SourceGenerator.References.Tycho.Modules;
using Tycho.Utils.SourceGenerator.References.Tycho.Structure;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class ModuleDefinitionTM : TemplateModelBase
    {
        public string Namespace { get; }

        public string[] ContainingTypes { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public SubmoduleTM[] Submodules { get; }

        public ModuleDefinitionTM(TychoDefinitionModel tychoDefinitionModel)
        {
            Namespace = tychoDefinitionModel.DefinitionType.Namespace;
            ContainingTypes = tychoDefinitionModel.DefinitionType.ContainingTypes.ToArray();
            Classes = new ClassesTM(this, tychoDefinitionModel);
            Interfaces = new InterfacesTM(this);
            Methods = new MethodsTM();
            Parameters = new ParametersTM();
            Submodules = tychoDefinitionModel.Submodules.Select(s => new SubmoduleTM(this, s)).ToArray();
        }

        internal class ClassesTM
        {
            public string ModuleClass { get; }
            public string EventDispatcherClass { get; }
            public string BaseClass { get; }
            public string ServiceCollectionServiceExtensionsClass { get; }
            public string ServiceProviderServiceExtensionsClass { get; }

            public ClassesTM(ModuleDefinitionTM owner, TychoDefinitionModel tychoDefinitionModel)
            {
                ModuleClass = tychoDefinitionModel.DefinitionType.Name;
                EventDispatcherClass = EventDispatcherSymbols.GetEventDispatcherClass(tychoDefinitionModel.DefinitionType.Name);
                BaseClass = owner.UseType(TychoModuleReference.TypeModel);
                ServiceCollectionServiceExtensionsClass = owner.UseType(ServiceCollectionServiceExtensionsReference.TypeModel);
                ServiceProviderServiceExtensionsClass = owner.UseType(ServiceProviderServiceExtensionsReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string ServiceCollectionInterface { get; }
            public string EventHandlingDispatcherInterface { get; }
            public string ModuleInstanceInterface { get; }

            public InterfacesTM(ModuleDefinitionTM owner)
            {
                ServiceCollectionInterface = owner.UseType(IServiceCollectionReference.TypeModel);
                EventHandlingDispatcherInterface = owner.UseType(IEventHandlingDispatcherReference.TypeModel);
                ModuleInstanceInterface = owner.UseType(IModuleInstanceReference.TypeModel);
            }
        }

        internal class MethodsTM
        {
            public string AutoSetupMethod { get; }
            public string AddTransientMethod { get; }
            public string GetRequiredServiceMethod { get; }

            public MethodsTM()
            {
                AutoSetupMethod = TychoModuleReference.AutoSetupMethodSignature.MethodName;
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

            public SubmoduleTM(ModuleDefinitionTM owner, TypeModel moduleType)
            {
                ModuleClass = owner.UseType(moduleType);
                FacadeInterface = ModuleFacadeSymbols.GetModuleFacadeInterface(ModuleClass);
                FacadeClass = ModuleFacadeSymbols.GetModuleFacadeClass(ModuleClass);
            }
        }
    }
}
