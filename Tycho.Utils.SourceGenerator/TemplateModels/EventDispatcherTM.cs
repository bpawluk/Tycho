using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.References;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class EventDispatcherTM : TemplateModelBase
    {
        public string Namespace { get; }

        public string[] ContainingTypes { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public StructsTM Structs { get; }

        public ExceptionsTM Exceptions { get; }

        public MethodsTM Methods { get; }

        public SymbolsTM Symbols { get; }

        public string[] Events { get; }

        public EventDispatcherTM(EventDispatcherModel eventDispatcherModel)
        {
            Namespace = eventDispatcherModel.DefinitionType.Namespace;
            ContainingTypes = eventDispatcherModel.DefinitionType.ContainingTypes.ToArray();
            Classes = new ClassesTM(this, eventDispatcherModel);
            Interfaces = new InterfacesTM(this);
            Structs = new StructsTM(this);
            Exceptions = new ExceptionsTM(this);
            Methods = new MethodsTM();
            Symbols = new SymbolsTM();
            Events = eventDispatcherModel.Events.Select(e => UseType(e)).ToArray();
        }

        internal class ClassesTM
        {
            public string EventDispatcherClass { get; }
            public string TaskClass { get; }
            public string EventContextClass { get; }

            public ClassesTM(EventDispatcherTM owner, EventDispatcherModel eventDispatcherModel)
            {
                EventDispatcherClass = $"{eventDispatcherModel.DefinitionType.Name}EventDispatcher";
                TaskClass = owner.UseType(TaskReference.TypeModel);
                EventContextClass = owner.UseType(EventContextReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string EventHandlingDispatcherInterface { get; }
            public string PayloadSerializerInterface { get; }
            public string EventHandlerInterface { get; }
            public string EventInterface { get; }

            public InterfacesTM(EventDispatcherTM owner)
            {
                EventHandlingDispatcherInterface = owner.UseType(IEventHandlingDispatcherReference.TypeModel);
                PayloadSerializerInterface = owner.UseType(IPayloadSerializerReference.TypeModel);
                EventHandlerInterface = owner.UseType(IEventHandlerReference.TypeModel);
                EventInterface = owner.UseType(IEventReference.TypeModel);
            }
        }

        internal class StructsTM
        {
            public string GuidStruct { get; }
            public string CancellationTokenStruct { get; }

            public StructsTM(EventDispatcherTM owner)
            {
                GuidStruct = owner.UseType(GuidReference.TypeModel);
                CancellationTokenStruct = owner.UseType(CancellationTokenReference.TypeModel);
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
            public string DeserializeMethod { get; }
            public string HandleAsyncMethod { get; }
            public string GetTypeMethod { get; }
            public string FullNameProperty { get; }

            public MethodsTM()
            {
                DeserializeMethod = IPayloadSerializerReference.DeserializeMethodName;
                HandleAsyncMethod = IEventHandlerReference.HandleAsyncMethodSignature.MethodName;
                GetTypeMethod = "GetType";
                FullNameProperty = "FullName";
            }
        }

        internal class SymbolsTM
        {
            public string PayloadSerializerField { get; }
            public string PayloadSerializerParameter { get; }
            public string DispatchMethod { get; }
            public string EventIdParameter { get; }
            public string EventPayloadParameter { get; }
            public string EventHandlerParameter { get; }
            public string CancellationTokenParameter { get; }
            public string CastHandlerVariable { get; }
            public string DispatchAsMethod { get; }
            public string TEventTypeParameter { get; }
            public string DeserializedPayloadVariable { get; }
            public string ContextVariable { get; }

            public SymbolsTM()
            {
                PayloadSerializerField = EventDispatcherSymbols.PayloadSerializerFieldName;
                PayloadSerializerParameter = EventDispatcherSymbols.PayloadSerializerParameterName;
                DispatchMethod = EventDispatcherSymbols.DispatchMethodName;
                EventIdParameter = EventDispatcherSymbols.EventIdParameterName;
                EventPayloadParameter = EventDispatcherSymbols.EventPayloadParameterName;
                EventHandlerParameter = EventDispatcherSymbols.EventHandlerParameterName;
                CancellationTokenParameter = EventDispatcherSymbols.CancellationTokenParameterName;
                CastHandlerVariable = EventDispatcherSymbols.CastHandlerVariableName;
                DispatchAsMethod = EventDispatcherSymbols.DispatchAsMethodName;
                TEventTypeParameter = EventDispatcherSymbols.TEventTypeParameterName;
                DeserializedPayloadVariable = EventDispatcherSymbols.DeserializedPayloadVariableName;
                ContextVariable = EventDispatcherSymbols.ContextVariableName;
            }
        }
    }
}
