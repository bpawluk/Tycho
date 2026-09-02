using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Events;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class ModulePublisherInterfaceTM : TemplateModelBase
    {
        public ContainingTypeTM[] ContainingTypes { get; }

        public string[] OwnerConstraints { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public string[] Events { get; }

        public ModulePublisherInterfaceTM(TychoPublisherModel tychoPublisherModel)
        {
            Namespace = tychoPublisherModel.DefinitionType.Namespace;
            ContainingTypes = UseContainingTypes(tychoPublisherModel.DefinitionType.ContainingTypes);
            OwnerConstraints = UseConstraintClauses(tychoPublisherModel.DefinitionType.TypeParameters).ToArray();
            Classes = new ClassesTM(this);
            Interfaces = new InterfacesTM(tychoPublisherModel);
            Methods = new MethodsTM();
            Parameters = new ParametersTM();
            Events = tychoPublisherModel.Events.Select(e => UseType(e)).ToArray();
        }

        internal class ClassesTM
        {
            public string TaskClass { get; }
            public string CancellationTokenClass { get; }

            public ClassesTM(ModulePublisherInterfaceTM owner)
            {
                TaskClass = owner.UseType(TaskReference.TypeModel);
                CancellationTokenClass = owner.UseType(CancellationTokenReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string PublisherInterface { get; }

            public InterfacesTM(TychoPublisherModel tychoPublisherModel)
            {
                var publisherInterfaceType = new GeneratedTypeModel(
                    tychoPublisherModel.DefinitionType,
                    PublisherSymbols.GetPublisherInterface(tychoPublisherModel.DefinitionType.Name));
                PublisherInterface = publisherInterfaceType.DeclarationName;
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
