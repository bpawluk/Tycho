using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.References;
using Tycho.Utils.SourceGenerator.References.Microsoft;
using Tycho.Utils.SourceGenerator.References.System;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class AppDefinitionTM : TemplateModelBase
    {
        public string Namespace { get; }

        public string[] ContainingTypes { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ExceptionsTM Exceptions { get; }

        public AppDefinitionTM(TychoDefinitionModel tychoDefinitionModel)
        {
            Namespace = tychoDefinitionModel.DefinitionType.Namespace;
            ContainingTypes = tychoDefinitionModel.DefinitionType.ContainingTypes.ToArray();
            Classes = new ClassesTM(this, tychoDefinitionModel);
            Interfaces = new InterfacesTM(this, tychoDefinitionModel);
            Methods = new MethodsTM();
            Exceptions = new ExceptionsTM(this);
        }

        internal class ClassesTM 
        {
            public string AppClass { get; }
            public string BaseClass { get; }
            public string FacadeClass { get; }
            public string EventDispatcherClass { get; }
            public string TaskClass { get; }
            public string ActionClass { get; }
            public string ServiceCollectionServiceExtensionsClass { get; }

            public ClassesTM(AppDefinitionTM owner, TychoDefinitionModel tychoDefinitionModel)
            {
                AppClass = tychoDefinitionModel.DefinitionType.Name;
                BaseClass = owner.UseType(TychoAppReference.TypeModel);
                FacadeClass = $"{AppClass}Facade";
                EventDispatcherClass = $"{AppClass}EventDispatcher";
                TaskClass = owner.UseType(TaskReference.TypeModel);
                ActionClass = owner.UseType(ActionReference.TypeModel);
                ServiceCollectionServiceExtensionsClass = owner.UseType(ServiceCollectionServiceExtensionsReference.TypeModel);
            }
        }

        internal class InterfacesTM 
        { 
            public string AppInterface { get; }
            public string ConfigurationInterface { get; }
            public string LoggingBuilderInterface { get; }
            public string ServiceCollectionInterface { get; }
            public string EventHandlingDispatcherInterface { get; }

            public InterfacesTM(AppDefinitionTM owner, TychoDefinitionModel tychoDefinitionModel)
            {
                AppInterface = $"I{tychoDefinitionModel.DefinitionType.Name}";
                ConfigurationInterface = owner.UseType(IConfigurationReference.TypeModel);
                LoggingBuilderInterface = owner.UseType(ILoggingBuilderReference.TypeModel);
                ServiceCollectionInterface = owner.UseType(IServiceCollectionReference.TypeModel);
                EventHandlingDispatcherInterface = owner.UseType(IEventHandlingDispatcherReference.TypeModel);
            }
        }

        internal class MethodsTM 
        { 
            public string WithConfigurationBaseMethod { get; }
            public string WithLoggingBaseMethod { get; }
            public string RunBaseMethod { get; }
            public string AutoSetupMethod { get; }
            public string AddTransientMethod { get; }

            public MethodsTM()
            {
                WithConfigurationBaseMethod = TychoAppReference.WithConfigurationBaseMethodSignature.MethodName;
                WithLoggingBaseMethod = TychoAppReference.WithLoggingBaseMethodSignature.MethodName;
                RunBaseMethod = TychoAppReference.RunBaseAsyncMethodSignature.MethodName;
                AutoSetupMethod = TychoAppReference.AutoSetupMethodSignature.MethodName;
                AddTransientMethod = ServiceCollectionServiceExtensionsReference.AddTransientMethodSignature.MethodName;
            }
        }

        internal class ExceptionsTM
        {
            public string ArgumentNullException { get; }
            public string InvalidOperationException { get; }

            public ExceptionsTM(AppDefinitionTM owner)
            {
                ArgumentNullException = owner.UseType(ArgumentNullExceptionReference.TypeModel);
                InvalidOperationException = owner.UseType(InvalidOperationExceptionReference.TypeModel);
            }
        }
    }
}
