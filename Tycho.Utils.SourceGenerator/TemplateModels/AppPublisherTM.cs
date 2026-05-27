using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Events;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class AppPublisherTM : TemplateModelBase
    {
        public string[] ContainingTypes { get; }

        public string[] OwnerConstraints { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public string[] Events { get; }

        public AppPublisherTM(TychoPublisherModel tychoPublisherModel)
        {
            Namespace = tychoPublisherModel.DefinitionType.Namespace;
            ContainingTypes = tychoPublisherModel.DefinitionType.ContainingTypeDeclarationSignatures.ToArray();
            OwnerConstraints = tychoPublisherModel.DefinitionType.TypeParameterConstraintClauses.ToArray();
            Classes = new ClassesTM(this, tychoPublisherModel);
            Interfaces = new InterfacesTM();
            Methods = new MethodsTM();
            Parameters = new ParametersTM();
            Events = tychoPublisherModel.Events.Select(e => UseType(e)).ToArray();
        }

        internal class ClassesTM
        {
            public string AppClass { get; }
            public string PublisherClassName { get; }
            public string PublisherClass { get; }
            public string PublisherBaseClass { get; }
            public string TaskClass { get; }
            public string CancellationTokenClass { get; }
            public string GenericPublisherClass { get; }

            public ClassesTM(AppPublisherTM owner, TychoPublisherModel tychoPublisherModel)
            {
                string appNameStem = tychoPublisherModel.DefinitionType.Name;
                string appTypeSuffix = tychoPublisherModel.DefinitionType.TypeParametersSuffix;
                AppClass = tychoPublisherModel.DefinitionType.ReferenceName;
                PublisherClassName = PublisherSymbols.GetPublisherClass(appNameStem);
                PublisherClass = $"{PublisherClassName}{appTypeSuffix}";
                PublisherBaseClass = owner.UseType(PublisherBaseReference.TypeModel);
                TaskClass = owner.UseType(TaskReference.TypeModel);
                CancellationTokenClass = owner.UseType(CancellationTokenReference.TypeModel);
                GenericPublisherClass = owner.UseType(IEventPublisherReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string PublisherInterface { get; }

            public InterfacesTM()
            {
                PublisherInterface = PublisherSymbols.PublisherInterface;
            }
        }

        internal class MethodsTM
        {
            public string PublishAsyncMethod { get; }

            public MethodsTM()
            {
                PublishAsyncMethod = PublisherBaseReference.PublishAsyncMethodSignature.MethodName;
            }
        }

        internal class ParametersTM
        {
            public string GenericPublisherParameter { get; }
            public string EventPayloadParameter { get; }
            public string CancellationTokenParameter { get; }

            public ParametersTM()
            {
                GenericPublisherParameter = PublisherSymbols.GenericPublisherParameter;
                EventPayloadParameter = PublisherSymbols.EventPayloadParameter;
                CancellationTokenParameter = PublisherSymbols.CancellationTokenParameter;
            }
        }
    }
}
