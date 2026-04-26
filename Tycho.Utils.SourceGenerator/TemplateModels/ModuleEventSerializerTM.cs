using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.References.Tycho.Events;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class ModuleEventSerializerTM : TemplateModelBase
    {
        public string Namespace { get; }

        public string[] ContainingTypes { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public string[] Events { get; }

        public ModuleEventSerializerTM(TychoEventSerializerModel tychoEventSerializerModel)
        {
            Namespace = tychoEventSerializerModel.DefinitionType.Namespace;
            ContainingTypes = tychoEventSerializerModel.DefinitionType.ContainingTypes.ToArray();
            Classes = new ClassesTM(this, tychoEventSerializerModel.DefinitionType.Name);
            Interfaces = new InterfacesTM(this);
            Methods = new MethodsTM();
            Parameters = new ParametersTM();
            Events = tychoEventSerializerModel.Events.Select(e => UseType(e)).ToArray();
        }

        internal class ClassesTM
        {
            public string EventSerializerClass { get; }
            public string EventSerializerBaseClass { get; }

            public ClassesTM(ModuleEventSerializerTM owner, string moduleClass)
            {
                EventSerializerClass = EventSerializerSymbols.GetEventSerializerClass(moduleClass);
                EventSerializerBaseClass = owner.UseType(EventSerializerBaseReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string PayloadSerializerInterface { get; }

            public InterfacesTM(ModuleEventSerializerTM owner)
            {
                PayloadSerializerInterface = owner.UseType(IPayloadSerializerReference.TypeModel);
            }
        }

        internal class MethodsTM
        {
            public string RegisterEventMethod { get; }

            public MethodsTM()
            {
                RegisterEventMethod = EventSerializerBaseReference.RegisterEventMethodSignature.MethodName;
            }
        }

        internal class ParametersTM
        {
            public string PayloadSerializerParameter { get; }

            public ParametersTM()
            {
                PayloadSerializerParameter = EventSerializerSymbols.PayloadSerializerParameter;
            }
        }
    }
}