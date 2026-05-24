using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.References.Microsoft;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Logging;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class AppExtensionsTM : TemplateModelBase
    {
        public string Namespace { get; }

        public string[] ContainingTypes { get; }

        public string[] OwnerConstraints { get; }

        public ClassesTM Classes { get; }

        public string TypeParametersSuffix { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public PropertiesTM Properties { get; }

        public ParametersTM Parameters { get; }

        public AppExtensionsTM(TychoExtensionsModel tychoExtensionsModel)
        {
            Namespace = tychoExtensionsModel.DefinitionType.Namespace;
            ContainingTypes = tychoExtensionsModel.DefinitionType.ContainingTypeDeclarationSignatures.ToArray();
            OwnerConstraints = tychoExtensionsModel.DefinitionType.TypeParameterConstraintClauses.ToArray();
            Classes = new ClassesTM(this, tychoExtensionsModel);
            TypeParametersSuffix = tychoExtensionsModel.DefinitionType.TypeParametersSuffix;
            Interfaces = new InterfacesTM(this);
            Methods = new MethodsTM(tychoExtensionsModel);
            Properties = new PropertiesTM();
            Parameters = new ParametersTM();
        }

        internal class ClassesTM
        {
            public string AppClass { get; }
            public string SetupExtensionsClass { get; }
            public string TaskClass { get; }
            public string LoggingConfigurationClass { get; }
            public string ServiceCollectionServiceExtensionsClass { get; }

            public ClassesTM(AppExtensionsTM owner, TychoExtensionsModel tychoExtensionsModel)
            {
                string appNameStem = tychoExtensionsModel.DefinitionType.NameWithArity;
                AppClass = tychoExtensionsModel.DefinitionType.ReferenceName;
                SetupExtensionsClass = AppExtensionsSymbols.GetAppSetupExtensionsClass(appNameStem);
                TaskClass = owner.UseType(TaskReference.TypeModel);
                LoggingConfigurationClass = owner.UseType(LoggingConfigurationReference.TypeModel);
                ServiceCollectionServiceExtensionsClass = owner.UseType(ServiceCollectionServiceExtensionsReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string HostApplicationBuilderInterface { get; }

            public InterfacesTM(AppExtensionsTM owner)
            {
                HostApplicationBuilderInterface = owner.UseType(IHostApplicationBuilderReference.TypeModel);
            }
        }

        internal class MethodsTM
        {
            public string AddAppMethod { get; }
            public string ConfigureLoggingMethod { get; }
            public string AddSingletonMethod { get; }
            public string WithConfigurationMethod { get; }
            public string WithLoggingMethod { get; }
            public string RunAsyncMethod { get; }
            public string ConfigureAwaitMethod { get; }

            public MethodsTM(TychoExtensionsModel tychoextensionsModel)
            {
                AddAppMethod = AppExtensionsSymbols.GetAddAppMethod(tychoextensionsModel.DefinitionType.Name);
                WithConfigurationMethod = AppSetupSymbols.WithConfigurationMethod;
                WithLoggingMethod = AppSetupSymbols.WithLoggingMethod;
                RunAsyncMethod = AppSetupSymbols.RunAsyncMethod;
                AddSingletonMethod = ServiceCollectionServiceExtensionsReference.AddSingletonMethodSignature.MethodName;
                ConfigureLoggingMethod = LoggingConfigurationReference.ConfigureLoggingMethodSignature.MethodName;
                ConfigureAwaitMethod = TaskReference.ConfigureAwaitMethodSignature.MethodName;
            }
        }

        internal class PropertiesTM
        {
            public string ConfigurationProperty { get; }
            public string ServicesProperty { get; }

            public PropertiesTM()
            {
                ConfigurationProperty = IHostApplicationBuilderReference.ConfigurationPropertyName;
                ServicesProperty = IHostApplicationBuilderReference.ServicesPropertyName;
            }
        }

        internal class ParametersTM
        {
            public string AppParameter { get; }
            public string BuilderParameter { get; }
            public string LoggingParameter { get; }

            public ParametersTM()
            {
                AppParameter = AppExtensionsSymbols.AppParameter;
                BuilderParameter = AppExtensionsSymbols.BuilderParameter;
                LoggingParameter = AppExtensionsSymbols.LoggingParameter;
            }
        }
    }
}
