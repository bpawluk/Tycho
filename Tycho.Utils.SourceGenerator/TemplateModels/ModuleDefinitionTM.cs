using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.References;
using Tycho.Utils.SourceGenerator.References.Microsoft;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class ModuleDefinitionTM : TemplateModelBase
    {
        public string Namespace { get; }

        public string[] ContainingTypes { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ModuleDefinitionTM(TychoDefinitionModel tychoDefinitionModel)
        {
            Namespace = tychoDefinitionModel.DefinitionType.Namespace;
            ContainingTypes = tychoDefinitionModel.DefinitionType.ContainingTypes.ToArray();
            Classes = new ClassesTM(this, tychoDefinitionModel);
            Interfaces = new InterfacesTM(this);
            Methods = new MethodsTM();
        }

        internal class ClassesTM
        {
            public string ModuleClass { get; }
            public string BaseClass { get; }
            public string EventDispatcherClass { get; }
            public string ServiceCollectionServiceExtensionsClass { get; }

            public ClassesTM(ModuleDefinitionTM owner, TychoDefinitionModel tychoDefinitionModel)
            {
                ModuleClass = tychoDefinitionModel.DefinitionType.Name;
                BaseClass = owner.UseType(TychoModuleReference.TypeModel);
                EventDispatcherClass = $"{ModuleClass}EventDispatcher";
                ServiceCollectionServiceExtensionsClass = owner.UseType(ServiceCollectionServiceExtensionsReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string ServiceCollectionInterface { get; }
            public string EventHandlingDispatcherInterface { get; }

            public InterfacesTM(ModuleDefinitionTM owner)
            {
                ServiceCollectionInterface = owner.UseType(IServiceCollectionReference.TypeModel);
                EventHandlingDispatcherInterface = owner.UseType(IEventHandlingDispatcherReference.TypeModel);
            }
        }

        internal class MethodsTM
        {
            public string AutoSetupMethod { get; }
            public string AddTransientMethod { get; }

            public MethodsTM()
            {
                AutoSetupMethod = TychoModuleReference.AutoSetupMethodSignature.MethodName;
                AddTransientMethod = ServiceCollectionServiceExtensionsReference.AddTransientMethodSignature.MethodName;
            }
        }
    }
}
