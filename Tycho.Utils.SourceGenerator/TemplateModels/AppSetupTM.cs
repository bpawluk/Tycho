using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.References;
using Tycho.Utils.SourceGenerator.References.Microsoft;
using Tycho.Utils.SourceGenerator.References.System;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class AppSetupTM : TemplateModelBase
    {
        public string Namespace { get; }

        public string[] ContainingTypes { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public AppSetupTM(TychoSetupModel tychoSetupModel)
        {
            Namespace = tychoSetupModel.DefinitionType.Namespace;
            ContainingTypes = tychoSetupModel.DefinitionType.ContainingTypes.ToArray();
            Classes = new ClassesTM(this, tychoSetupModel);
            Interfaces = new InterfacesTM(this);
            Methods = new MethodsTM(tychoSetupModel);
        }

        internal class ClassesTM
        {
            public string AppClass { get; }
            public string SetupExtensionsClass { get; }
            public string TaskClass { get; }
            public string LoggingConfigurationClass { get; }
            public string ServiceCollectionServiceExtensionsClass { get; }

            public ClassesTM(AppSetupTM owner, TychoSetupModel tychoSetupModel)
            {
                AppClass = tychoSetupModel.DefinitionType.Name;
                SetupExtensionsClass = $"{AppClass}SetupExtensions";
                TaskClass = owner.UseType(TaskReference.TypeModel);
                LoggingConfigurationClass = owner.UseType(LoggingConfigurationReference.TypeModel);
                ServiceCollectionServiceExtensionsClass = owner.UseType(ServiceCollectionServiceExtensionsReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string HostApplicationBuilderInterface { get; }

            public InterfacesTM(AppSetupTM owner)
            {
                HostApplicationBuilderInterface = owner.UseType(IHostApplicationBuilderReference.TypeModel);
            }
        }

        internal class MethodsTM
        {
            public string AddAppMethod { get; }
            public string ConfigureLoggingMethod { get; }
            public string AddSingletonMethod { get; }

            public MethodsTM(TychoSetupModel tychoSetupModel)
            {
                AddAppMethod = $"Add{tychoSetupModel.DefinitionType.Name}";
                ConfigureLoggingMethod = LoggingConfigurationReference.ConfigureLoggingMethodSignature.MethodName;
                AddSingletonMethod = ServiceCollectionServiceExtensionsReference.AddSingletonMethodSignature.MethodName;
            }
        }
    }
}
