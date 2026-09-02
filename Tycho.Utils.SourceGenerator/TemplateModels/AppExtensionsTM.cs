using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.References.Microsoft;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Apps;
using Tycho.Utils.SourceGenerator.References.Tycho.Hosting;
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

        public AppExtensionsTM(TychoExtensionsModel model)
        {
            Namespace = model.DefinitionType.Namespace;
            TypeParameters = model.DefinitionType.ContainingTypes
                .SelectMany(type => type.TypeParameters.Select(parameter => parameter.Name))
                .Concat(model.DefinitionType.TypeParameters.Select(parameter => parameter.Name))
                .Distinct()
                .ToArray();
            TypeParametersSuffix = TypeParameters.Length == 0
                ? string.Empty
                : $"<{string.Join(", ", TypeParameters)}>";
            TypeParametersConstraints = UseConstraintClauses(
                    model.DefinitionType.ContainingTypes
                        .SelectMany(type => type.TypeParameters)
                        .Concat(model.DefinitionType.TypeParameters))
                .Distinct()
                .ToArray();

            Classes = new ClassesTM(this, model);
            Interfaces = new InterfacesTM(this, model);
            Methods = new MethodsTM(model);
            Properties = new PropertiesTM();
            Parameters = new ParametersTM();
        }

        internal class ClassesTM
        {
            public string AppClass { get; }
            public string AppBuilderClass { get; }
            public string SetupExtensionsClass { get; }
            public string AppHostedLifecycleServiceClass { get; }
            public string ArgumentNullExceptionClass { get; }
            public string EnumerableClass { get; }
            public string InvalidOperationExceptionClass { get; }
            public string ServiceCollectionHostedServiceExtensionsClass { get; }
            public string ServiceCollectionServiceExtensionsClass { get; }

            public ClassesTM(AppExtensionsTM owner, TychoExtensionsModel model)
            {
                string appName = model.DefinitionType.Name;
                AppClass = model.DefinitionType.FullDeclarationName;
                AppBuilderClass = AppBuilderSymbols.GetAppBuilderClass(appName);
                SetupExtensionsClass = AppExtensionsSymbols.GetAppSetupExtensionsClass(appName);
                AppHostedLifecycleServiceClass = owner.UseType(AppHostedLifecycleServiceReference.TypeModel);
                ArgumentNullExceptionClass = owner.UseType(ArgumentNullExceptionReference.TypeModel);
                EnumerableClass = owner.UseType(EnumerableReference.TypeModel);
                InvalidOperationExceptionClass = owner.UseType(InvalidOperationExceptionReference.TypeModel);
                ServiceCollectionHostedServiceExtensionsClass = owner.UseType(ServiceCollectionHostedServiceExtensionsReference.TypeModel);
                ServiceCollectionServiceExtensionsClass = owner.UseType(ServiceCollectionServiceExtensionsReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string HostApplicationBuilderInterface { get; }
            public string FacadeInterface { get; }

            public InterfacesTM(AppExtensionsTM owner, TychoExtensionsModel model)
            {
                HostApplicationBuilderInterface = owner.UseType(IHostApplicationBuilderReference.TypeModel);
                FacadeInterface = AppFacadeSymbols.GetAppFacadeInterface(
                    model.DefinitionType.Name,
                    model.DefinitionType.TypeParametersSuffix);
            }
        }

        internal class MethodsTM
        {
            public string AddAppMethod { get; }
            public string AddHostedServiceMethod { get; }
            public string AddSingletonMethod { get; }
            public string AnyMethod { get; }
            public string BuildMethod { get; }
            public string CreateAppBuilderMethod { get; }
            public string CreateAppBuilderBaseMethod { get; }

            public MethodsTM(TychoExtensionsModel model)
            {
                AddAppMethod = AppExtensionsSymbols.GetAddAppMethod(model.DefinitionType.Name);
                AddHostedServiceMethod = ServiceCollectionHostedServiceExtensionsReference.AddHostedServiceMethodSignature.MethodName;
                AddSingletonMethod = ServiceCollectionServiceExtensionsReference.AddSingletonMethodSignature.MethodName;
                AnyMethod = EnumerableReference.AnyMethodSignature.MethodName;
                BuildMethod = AppBuilderSymbols.BuildMethod;
                CreateAppBuilderMethod = AppExtensionsSymbols.CreateAppBuilderMethod;
                CreateAppBuilderBaseMethod = TychoAppReference.CreateAppBuilderBaseMethodSignature.MethodName;
            }
        }

        internal class PropertiesTM
        {
            public string ServicesProperty { get; } = IHostApplicationBuilderReference.ServicesPropertyName;
            public string ServiceTypeProperty { get; } = ServiceDescriptorReference.ServiceTypePropertyName;
        }

        internal class ParametersTM
        {
            public string AppParameter { get; } = AppExtensionsSymbols.AppParameter;
            public string AppDefinitionParameter { get; } = AppExtensionsSymbols.AppDefinitionParameter;
            public string AppBuilderBaseParameter { get; } = AppExtensionsSymbols.AppBuilderBaseParameter;
            public string AppBuilderVariable { get; } = AppExtensionsSymbols.AppBuilderVariable;
            public string BuilderParameter { get; } = AppExtensionsSymbols.BuilderParameter;
            public string ServiceDescriptorParameter { get; } = AppExtensionsSymbols.ServiceDescriptorParameter;
            public string ServiceProviderParameter { get; } = AppExtensionsSymbols.ServiceProviderParameter;
        }
    }
}
