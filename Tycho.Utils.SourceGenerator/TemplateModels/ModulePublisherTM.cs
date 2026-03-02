using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Events;
using Tycho.Utils.SourceGenerator.References.Tycho.Modules;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class ModulePublisherTM : TemplateModelBase
    {
        public string Namespace { get; }

        public string[] ContainingTypes { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public string[] Events { get; }

        public ModulePublisherTM(TychoPublisherModel tychoPublisherModel)
        {
            Namespace = tychoPublisherModel.DefinitionType.Namespace;
            ContainingTypes = tychoPublisherModel.DefinitionType.ContainingTypes.ToArray();
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
            public string PublisherClass { get; }
            public string PublisherBaseClass { get; }
            public string TaskClass { get; }
            public string CancellationTokenClass { get; }
            public string GenericPublisherClass { get; }

            public ClassesTM(ModulePublisherTM owner, TychoPublisherModel tychoPublisherModel)
            {
                ModuleClass = tychoPublisherModel.DefinitionType.Name;
                ModuleBaseClass = owner.UseType(TychoModuleReference.TypeModel);
                PublisherClass = PublisherSymbols.PublisherClass;
                PublisherBaseClass = owner.UseType(PublisherBaseReference.TypeModel);
                TaskClass = owner.UseType(TaskReference.TypeModel);
                CancellationTokenClass = owner.UseType(CancellationTokenReference.TypeModel);
                GenericPublisherClass = owner.UseType(IGenericPublisherReference.TypeModel);
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
