using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Events;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class ModuleEventSerializerTM : TemplateModelBase
    {
        public ContainingTypeTM[] ContainingTypes { get; }

        public string[] OwnerConstraints { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public string[] Events { get; }

        public ModuleEventSerializerTM(TychoEventSerializerModel tychoEventSerializerModel)
        {
            Namespace = tychoEventSerializerModel.DefinitionType.Namespace;
            ContainingTypes = UseContainingTypes(tychoEventSerializerModel.DefinitionType.ContainingTypes);
            OwnerConstraints = UseConstraintClauses(tychoEventSerializerModel.DefinitionType.TypeParameters).ToArray();
            Classes = new ClassesTM(this, tychoEventSerializerModel.DefinitionType);
            Interfaces = new InterfacesTM(this);
            Methods = new MethodsTM();
            Parameters = new ParametersTM();
            Events = tychoEventSerializerModel.Events.Select(e => e.FullReferenceName).ToArray();
        }

        internal class ClassesTM
        {
            public string EventSerializerClass { get; }
            public string EventSerializerClassWithTypeParams { get; }
            public string EventSerializerBaseClass { get; }

            public ClassesTM(ModuleEventSerializerTM owner, TypeDefinitionModel moduleType)
            {
                var eventSerializerType = new GeneratedTypeModel(
                    moduleType,
                    EventSerializerSymbols.GetEventSerializerClass(moduleType.Name));
                EventSerializerClass = eventSerializerType.Identifier;
                EventSerializerClassWithTypeParams = eventSerializerType.DeclarationName;
                EventSerializerBaseClass = EventSerializerBaseReference.TypeModel.FullReferenceName;
            }
        }

        internal class InterfacesTM
        {
            public string PayloadSerializerInterface { get; }

            public InterfacesTM(ModuleEventSerializerTM owner)
            {
                PayloadSerializerInterface = IPayloadSerializerReference.TypeModel.FullReferenceName;
            }
        }

        internal class MethodsTM
        {
            public string RegisterEventMethod { get; }

            public MethodsTM()
            {
                RegisterEventMethod = EventSerializerBaseReference.RegisterEventMethodName;
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
