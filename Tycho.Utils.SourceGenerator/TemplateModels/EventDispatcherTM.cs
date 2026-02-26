using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Events;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class EventDispatcherTM : TemplateModelBase
    {
        public string Namespace { get; }

        public string[] ContainingTypes { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public ExceptionsTM Exceptions { get; }

        public MethodsTM Methods { get; }

        public PropertiesTM Properties { get; }

        public ParametersTM Parameters { get; }

        public string[] Events { get; }

        public EventDispatcherTM(EventDispatcherModel eventDispatcherModel)
        {
            Namespace = eventDispatcherModel.DefinitionType.Namespace;
            ContainingTypes = eventDispatcherModel.DefinitionType.ContainingTypes.ToArray();
            Classes = new ClassesTM(this, eventDispatcherModel);
            Interfaces = new InterfacesTM(this);
            Exceptions = new ExceptionsTM(this);
            Methods = new MethodsTM();
            Properties = new PropertiesTM();
            Parameters = new ParametersTM();
            Events = eventDispatcherModel.Events.Select(e => UseType(e)).ToArray();
        }

        internal class ClassesTM
        {
            public string EventDispatcherClass { get; }
            public string EventDispatcherBaseClass { get; }
            public string TaskClass { get; }
            public string EventContextClass { get; }
            public string GuidClass { get; }
            public string CancellationTokenClass { get; }
            public string ObjectClass { get; }

            public ClassesTM(EventDispatcherTM owner, EventDispatcherModel eventDispatcherModel)
            {
                EventDispatcherClass = EventDispatcherSymbols.GetEventDispatcherClass(eventDispatcherModel.DefinitionType.Name);
                EventDispatcherBaseClass = owner.UseType(EventDispatcherBaseReference.TypeModel);
                TaskClass = owner.UseType(TaskReference.TypeModel);
                EventContextClass = owner.UseType(EventContextReference.TypeModel);
                GuidClass = owner.UseType(GuidReference.TypeModel);
                CancellationTokenClass = owner.UseType(CancellationTokenReference.TypeModel);
                ObjectClass = owner.UseType(ObjectReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string PayloadSerializerInterface { get; }
            public string EventHandlerInterface { get; }
            public string EventInterface { get; }

            public InterfacesTM(EventDispatcherTM owner)
            {
                PayloadSerializerInterface = owner.UseType(IPayloadSerializerReference.TypeModel);
                EventHandlerInterface = owner.UseType(IEventHandlerReference.TypeModel);
                EventInterface = owner.UseType(IEventReference.TypeModel);
            }
        }

        internal class ExceptionsTM
        {
            public string InvalidOperationException { get; }

            public ExceptionsTM(EventDispatcherTM owner)
            {
                InvalidOperationException = owner.UseType(InvalidOperationExceptionReference.TypeModel);
            }
        }

        internal class MethodsTM
        {
            public string GetTypeMethod { get; }
            public string DispatchAsMethod { get; }
            public string DispatchMethod { get; }

            public MethodsTM()
            {
                GetTypeMethod = ObjectReference.GetTypeMethodSignature.MethodName;
                DispatchAsMethod = EventDispatcherBaseReference.DispatchAsMethodSignature.MethodName;
                DispatchMethod = EventDispatcherSymbols.DispatchMethod;
            }
        }

        internal class PropertiesTM
        {
            public string FullNameProperty { get; }

            public PropertiesTM()
            {
                FullNameProperty = TypeReference.FullNamePropertyName;
            }
        }

        internal class ParametersTM
        {
            public string EventIdParameter { get; }
            public string EventPayloadParameter { get; }
            public string EventHandlerParameter { get; }
            public string CancellationTokenParameter { get; }
            public string PayloadSerializerParameter { get; }

            public ParametersTM()
            {
                PayloadSerializerParameter = EventDispatcherSymbols.PayloadSerializerParameter;
                EventIdParameter = EventDispatcherSymbols.EventIdParameter;
                EventPayloadParameter = EventDispatcherSymbols.EventPayloadParameter;
                EventHandlerParameter = EventDispatcherSymbols.EventHandlerParameter;
                CancellationTokenParameter = EventDispatcherSymbols.CancellationTokenParameter;
            }
        }
    }
}
