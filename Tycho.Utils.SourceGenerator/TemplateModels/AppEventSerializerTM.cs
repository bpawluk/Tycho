using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Events;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class AppEventSerializerTM : TemplateModelBase
    {
        public ContainingTypeTM[] ContainingTypes { get; }

        public string[] OwnerConstraints { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public string[] Events { get; }

        public AppEventSerializerTM(TychoEventSerializerModel tychoEventSerializerModel)
        {
            Namespace = tychoEventSerializerModel.DefinitionType.Namespace;
            ContainingTypes = UseContainingTypes(tychoEventSerializerModel.DefinitionType.ContainingTypes);
            OwnerConstraints = UseConstraintClauses(tychoEventSerializerModel.DefinitionType.TypeParameters).ToArray();
            Classes = new ClassesTM(this, tychoEventSerializerModel.DefinitionType);
            Interfaces = new InterfacesTM(this);
            Methods = new MethodsTM();
            Parameters = new ParametersTM();
            Events = tychoEventSerializerModel.Events.Select(e => UseType(e)).ToArray();
        }

        internal class ClassesTM
        {
            public string EventSerializerClassWithTypeParams { get; }
            public string EventSerializerClass { get; }
            public string EventSerializerBaseClass { get; }

            public ClassesTM(AppEventSerializerTM owner, TypeDefinitionModel appType)
            {
                var eventSerializerType = new GeneratedTypeModel(
                    appType,
                    EventSerializerSymbols.GetEventSerializerClass(appType.Name));
                EventSerializerClass = eventSerializerType.Identifier;
                EventSerializerClassWithTypeParams = eventSerializerType.DeclarationName;
                EventSerializerBaseClass = owner.UseType(EventSerializerBaseReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string PayloadSerializerInterface { get; }

            public InterfacesTM(AppEventSerializerTM owner)
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
