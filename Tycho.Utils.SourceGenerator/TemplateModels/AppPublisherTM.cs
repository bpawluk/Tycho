using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Events;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class AppPublisherTM : TemplateModelBase
    {
        public ContainingTypeTM[] ContainingTypes { get; }

        public string[] OwnerConstraints { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public string[] Events { get; }

        public AppPublisherTM(TychoPublisherModel tychoPublisherModel)
        {
            Namespace = tychoPublisherModel.DefinitionType.Namespace;
            ContainingTypes = UseContainingTypes(tychoPublisherModel.DefinitionType.ContainingTypes);
            OwnerConstraints = UseConstraintClauses(tychoPublisherModel.DefinitionType.TypeParameters).ToArray();
            Classes = new ClassesTM(this, tychoPublisherModel);
            Interfaces = new InterfacesTM(tychoPublisherModel);
            Methods = new MethodsTM();
            Parameters = new ParametersTM();
            Events = tychoPublisherModel.Events.Select(e => UseType(e)).ToArray();
        }

        internal class ClassesTM
        {
            public string PublisherClass { get; }
            public string PublisherClassWithTypeParams { get; }
            public string PublisherBaseClass { get; }
            public string TaskClass { get; }
            public string CancellationTokenClass { get; }
            public string GenericPublisherClass { get; }

            public ClassesTM(AppPublisherTM owner, TychoPublisherModel tychoPublisherModel)
            {
                string appNameStem = tychoPublisherModel.DefinitionType.Name;
                PublisherClass = PublisherSymbols.GetPublisherClass(appNameStem);
                PublisherClassWithTypeParams = PublisherSymbols.GetPublisherClass(appNameStem, tychoPublisherModel.DefinitionType.TypeParametersSuffix);
                PublisherBaseClass = owner.UseType(PublisherBaseReference.TypeModel);
                TaskClass = owner.UseType(TaskReference.TypeModel);
                CancellationTokenClass = owner.UseType(CancellationTokenReference.TypeModel);
                GenericPublisherClass = owner.UseType(IEventPublisherReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string PublisherInterface { get; }

            public InterfacesTM(TychoPublisherModel tychoPublisherModel)
            {
                PublisherInterface = PublisherSymbols.GetPublisherInterface(tychoPublisherModel.DefinitionType.Name, tychoPublisherModel.DefinitionType.TypeParametersSuffix);
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
