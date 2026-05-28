using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Apps;
using Tycho.Utils.SourceGenerator.References.Tycho.Events;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class AppPublisherInterfaceTM : TemplateModelBase
    {
        public string[] ContainingTypes { get; }

        public string[] OwnerConstraints { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public string[] Events { get; }

        public AppPublisherInterfaceTM(TychoPublisherModel tychoPublisherModel)
        {
            Namespace = tychoPublisherModel.DefinitionType.Namespace;
            ContainingTypes = UseContainingTypeDeclarations(tychoPublisherModel.DefinitionType);
            OwnerConstraints = UseConstraintClauses(tychoPublisherModel.DefinitionType.TypeParameters).ToArray();
            Classes = new ClassesTM(this, tychoPublisherModel);
            Interfaces = new InterfacesTM();
            Methods = new MethodsTM();
            Parameters = new ParametersTM();
            Events = tychoPublisherModel.Events.Select(e => UseType(e)).ToArray();
        }

        internal class ClassesTM
        {
            public string AppClass { get; }
            public string AppBaseClass { get; }
            public string TaskClass { get; }
            public string CancellationTokenClass { get; }

            public ClassesTM(AppPublisherInterfaceTM owner, TychoPublisherModel tychoPublisherModel)
            {
                AppClass = tychoPublisherModel.DefinitionType.DeclarationName;
                AppBaseClass = owner.UseType(TychoAppReference.TypeModel);
                TaskClass = owner.UseType(TaskReference.TypeModel);
                CancellationTokenClass = owner.UseType(CancellationTokenReference.TypeModel);
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
            public string EventPayloadParameter { get; }
            public string CancellationTokenParameter { get; }

            public ParametersTM()
            {
                EventPayloadParameter = PublisherSymbols.EventPayloadParameter;
                CancellationTokenParameter = PublisherSymbols.CancellationTokenParameter;
            }
        }
    }
}
