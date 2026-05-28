using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Events;
using Tycho.Utils.SourceGenerator.References.Tycho.Modules;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class ModulePublisherInterfaceTM : TemplateModelBase
    {
        public string[] ContainingTypes { get; }

        public string[] OwnerConstraints { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public string[] Events { get; }

        public ModulePublisherInterfaceTM(TychoPublisherModel tychoPublisherModel)
        {
            Namespace = tychoPublisherModel.DefinitionType.Namespace;
            ContainingTypes = tychoPublisherModel.DefinitionType.ContainingTypeDeclarationSignatures.ToArray();
            OwnerConstraints = UseConstraintClauses(tychoPublisherModel.DefinitionType.TypeParameters).ToArray();
            Classes = new ClassesTM(this, tychoPublisherModel);
            Interfaces = new InterfacesTM();
            Methods = new MethodsTM();
            Parameters = new ParametersTM();
            Events = tychoPublisherModel.Events.Select(e => UseType(e)).ToArray();
        }

        internal class ClassesTM
        {
            public string ModuleClass { get; }
            public string ModuleBaseClass { get; }
            public string TaskClass { get; }
            public string CancellationTokenClass { get; }

            public ClassesTM(ModulePublisherInterfaceTM owner, TychoPublisherModel tychoPublisherModel)
            {
                ModuleClass = tychoPublisherModel.DefinitionType.DeclarationName;
                ModuleBaseClass = owner.UseType(TychoModuleReference.TypeModel);
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
