using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.References.Microsoft;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Apps;
using Tycho.Utils.SourceGenerator.References.Tycho.Logging;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class AppExtensionsTM : TemplateModelBase
    {
        public string[] TypeParameters { get; }

        public string TypeParametersSuffix { get; }

        public string[] TypeParametersConstraints { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public PropertiesTM Properties { get; }

        public ParametersTM Parameters { get; }

        public AppExtensionsTM(TychoExtensionsModel tychoExtensionsModel)
        {
            Namespace = tychoExtensionsModel.DefinitionType.Namespace;
            TypeParameters = tychoExtensionsModel.DefinitionType.ContainingTypes
                .SelectMany(containingType => containingType.TypeParameters.Select(typeParameter => typeParameter.Name))
                .Concat(tychoExtensionsModel.DefinitionType.TypeParameters.Select(typeParameter => typeParameter.Name))
                .Distinct()
                .ToArray();
            TypeParametersSuffix = TypeParameters.Length == 0 ? string.Empty : $"<{string.Join(", ", TypeParameters)}>";
            TypeParametersConstraints = UseConstraintClauses(
                tychoExtensionsModel.DefinitionType.ContainingTypes
                    .SelectMany(containingType => containingType.TypeParameters)
                    .Concat(tychoExtensionsModel.DefinitionType.TypeParameters))
                .Distinct()
                .ToArray();
            Classes = new ClassesTM(this, tychoExtensionsModel);
            Interfaces = new InterfacesTM(this, tychoExtensionsModel);
            Methods = new MethodsTM(tychoExtensionsModel);
            Properties = new PropertiesTM();
            Parameters = new ParametersTM();
        }

        internal class ClassesTM
        {
            public string AppClass { get; }
            public string SetupExtensionsClass { get; }
            public string FacadeClass { get; }
            public string TaskClass { get; }
            public string ActionClass { get; }
            public string LoggingConfigurationClass { get; }
            public string ServiceCollectionServiceExtensionsClass { get; }

            public ClassesTM(AppExtensionsTM owner, TychoExtensionsModel tychoExtensionsModel)
            {
                string appNameStem = tychoExtensionsModel.DefinitionType.Name;
                string appTypeSuffix = tychoExtensionsModel.DefinitionType.TypeParametersSuffix;
                AppClass = tychoExtensionsModel.DefinitionType.FullDeclarationName;
                SetupExtensionsClass = AppExtensionsSymbols.GetAppSetupExtensionsClass(appNameStem);
                FacadeClass = AppFacadeSymbols.GetAppFacadeClass(appNameStem, appTypeSuffix);
                TaskClass = owner.UseType(TaskReference.TypeModel);
                ActionClass = owner.UseType(ActionReference.TypeModel);
                LoggingConfigurationClass = owner.UseType(LoggingConfigurationReference.TypeModel);
                ServiceCollectionServiceExtensionsClass = owner.UseType(ServiceCollectionServiceExtensionsReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string HostApplicationBuilderInterface { get; }
            public string FacadeInterface { get; }
            public string ConfigurationInterface { get; }
            public string LoggingBuilderInterface { get; }

            public InterfacesTM(AppExtensionsTM owner, TychoExtensionsModel tychoExtensionsModel)
            {
                HostApplicationBuilderInterface = owner.UseType(IHostApplicationBuilderReference.TypeModel);
                FacadeInterface = AppFacadeSymbols.GetAppFacadeInterface(tychoExtensionsModel.DefinitionType.Name, tychoExtensionsModel.DefinitionType.TypeParametersSuffix);
                ConfigurationInterface = owner.UseType(IConfigurationReference.TypeModel);
                LoggingBuilderInterface = owner.UseType(ILoggingBuilderReference.TypeModel);
            }
        }

        internal class MethodsTM
        {
            public string AddAppMethod { get; }
            public string ConfigureLoggingMethod { get; }
            public string AddSingletonMethod { get; }
            public string WithConfigurationMethod { get; }
            public string WithLoggingMethod { get; }
            public string WithConfigurationBaseMethod { get; }
            public string WithLoggingBaseMethod { get; }
            public string RunBaseMethod { get; }
            public string RunAsyncMethod { get; }
            public string ConfigureAwaitMethod { get; }

            public MethodsTM(TychoExtensionsModel tychoextensionsModel)
            {
                AddAppMethod = AppExtensionsSymbols.GetAddAppMethod(tychoextensionsModel.DefinitionType.Name);
                WithConfigurationMethod = AppSetupSymbols.WithConfigurationMethod;
                WithLoggingMethod = AppSetupSymbols.WithLoggingMethod;
                WithConfigurationBaseMethod = TychoAppReference.WithConfigurationBaseMethodSignature.MethodName;
                WithLoggingBaseMethod = TychoAppReference.WithLoggingBaseMethodSignature.MethodName;
                RunBaseMethod = TychoAppReference.RunBaseAsyncMethodSignature.MethodName;
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
            public string GlobalConfigurationParameter { get; }
            public string LoggingSetupParameter { get; }

            public ParametersTM()
            {
                AppParameter = AppExtensionsSymbols.AppParameter;
                BuilderParameter = AppExtensionsSymbols.BuilderParameter;
                LoggingParameter = AppExtensionsSymbols.LoggingParameter;
                GlobalConfigurationParameter = AppSetupSymbols.GlobalConfigurationParameter;
                LoggingSetupParameter = AppSetupSymbols.LoggingSetupParameter;
            }
        }
    }
}
