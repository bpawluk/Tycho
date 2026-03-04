using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.Microsoft;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Apps;
using Tycho.Utils.SourceGenerator.References.Tycho.Modules;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class AppSetupTM : TemplateModelBase
    {
        public string Namespace { get; }

        public string[] ContainingTypes { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersVM Parameters { get; }

        public ExceptionsTM Exceptions { get; }

        public SubmoduleTM[] Submodules { get; }

        public AppSetupTM(TychoSetupModel tychoSetupModel)
        {
            Namespace = tychoSetupModel.DefinitionType.Namespace;
            ContainingTypes = tychoSetupModel.DefinitionType.ContainingTypes.ToArray();
            Classes = new ClassesTM(this, tychoSetupModel);
            Interfaces = new InterfacesTM(this, tychoSetupModel);
            Methods = new MethodsTM();
            Parameters = new ParametersVM();
            Exceptions = new ExceptionsTM(this);
            Submodules = tychoSetupModel.Submodules.Select(s => new SubmoduleTM(this, s)).ToArray();
        }

        internal class ClassesTM 
        {
            public string AppClass { get; }
            public string FacadeClass { get; }
            public string PublisherClass { get; }
            public string BaseClass { get; }
            public string TaskClass { get; }
            public string ActionClass { get; }
            public string ServiceCollectionServiceExtensionsClass { get; }
            public string ServiceProviderServiceExtensionsClass { get; }

            public ClassesTM(AppSetupTM owner, TychoSetupModel tychoSetupModel)
            {
                AppClass = tychoSetupModel.DefinitionType.Name;
                FacadeClass = AppFacadeSymbols.GetAppFacadeClass(AppClass);
                PublisherClass = PublisherSymbols.GetPublisherClass(AppClass);
                BaseClass = owner.UseType(TychoAppReference.TypeModel);
                TaskClass = owner.UseType(TaskReference.TypeModel);
                ActionClass = owner.UseType(ActionReference.TypeModel);
                ServiceCollectionServiceExtensionsClass = owner.UseType(ServiceCollectionServiceExtensionsReference.TypeModel);
                ServiceProviderServiceExtensionsClass = owner.UseType(ServiceProviderServiceExtensionsReference.TypeModel);
            }
        }

        internal class InterfacesTM 
        { 
            public string FacadeInterface { get; }
            public string PublisherInterface { get; }
            public string ConfigurationInterface { get; }
            public string LoggingBuilderInterface { get; }
            public string ServiceCollectionInterface { get; }
            public string ModuleInstanceInterface { get; }

            public InterfacesTM(AppSetupTM owner, TychoSetupModel tychoSetupModel)
            {
                FacadeInterface = AppFacadeSymbols.GetAppFacadeInterface(tychoSetupModel.DefinitionType.Name);
                PublisherInterface = PublisherSymbols.PublisherInterface;
                ConfigurationInterface = owner.UseType(IConfigurationReference.TypeModel);
                LoggingBuilderInterface = owner.UseType(ILoggingBuilderReference.TypeModel);
                ServiceCollectionInterface = owner.UseType(IServiceCollectionReference.TypeModel);
                ModuleInstanceInterface = owner.UseType(IModuleReference.TypeModel);
            }
        }

        internal class MethodsTM 
        { 
            public string WithConfigurationBaseMethod { get; }
            public string WithLoggingBaseMethod { get; }
            public string RunBaseMethod { get; }
            public string AutoSetupMethod { get; }
            public string AddTransientMethod { get; }
            public string ConfigureAwaitMethod { get; }
            public string WithConfigurationMethod { get; }
            public string WithLoggingMethod { get; }
            public string RunAsyncMethod { get; }
            public string GetRequiredServiceMethod { get; }

            public MethodsTM()
            {
                WithConfigurationBaseMethod = TychoAppReference.WithConfigurationBaseMethodSignature.MethodName;
                WithLoggingBaseMethod = TychoAppReference.WithLoggingBaseMethodSignature.MethodName;
                RunBaseMethod = TychoAppReference.RunBaseAsyncMethodSignature.MethodName;
                AutoSetupMethod = TychoAppReference.AutoSetupMethodSignature.MethodName;
                AddTransientMethod = ServiceCollectionServiceExtensionsReference.AddTransientMethodSignature.MethodName;
                ConfigureAwaitMethod = TaskReference.ConfigureAwaitMethodSignature.MethodName; 
                WithConfigurationMethod = AppSetupSymbols.WithConfigurationMethod;
                WithLoggingMethod = AppSetupSymbols.WithLoggingMethod;
                RunAsyncMethod = AppSetupSymbols.RunAsyncMethod;
                GetRequiredServiceMethod = ServiceProviderServiceExtensionsReference.GetRequiredServiceMethodSignature.MethodName;
            }
        }

        internal class ExceptionsTM
        {
            public string ArgumentNullException { get; }
            public string InvalidOperationException { get; }

            public ExceptionsTM(AppSetupTM owner)
            {
                ArgumentNullException = owner.UseType(ArgumentNullExceptionReference.TypeModel);
                InvalidOperationException = owner.UseType(InvalidOperationExceptionReference.TypeModel);
            }
        }

        internal class ParametersVM
        {
            public string GlobalConfigurationParameter { get; }
            public string LoggingSetupParameter { get; }
            public string AppParameter { get; }
            public string ProviderParameter { get; }

            public ParametersVM()
            {
                GlobalConfigurationParameter = AppSetupSymbols.GlobalConfigurationParameter;
                LoggingSetupParameter = AppSetupSymbols.LoggingSetupParameter;
                AppParameter = AppSetupSymbols.AppParameter;
                ProviderParameter = AppSetupSymbols.ProviderParameter;
            }
        }

        internal class SubmoduleTM
        {
            public string ModuleClass { get; }
            public string FacadeInterface { get; }
            public string FacadeClass { get; }

            public SubmoduleTM(AppSetupTM owner, TypeModel moduleType)
            {
                ModuleClass = owner.UseType(moduleType);
                FacadeInterface = ModuleFacadeSymbols.GetModuleFacadeInterface(ModuleClass);
                FacadeClass = ModuleFacadeSymbols.GetModuleFacadeClass(ModuleClass);
            }
        }
    }
}
