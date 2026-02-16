using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.References;
using Tycho.Utils.SourceGenerator.References.System;

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

        public string[] Events { get; }

        public EventDispatcherTM(EventDispatcherModel eventDispatcherModel)
        {
            Namespace = eventDispatcherModel.DefinitionType.Namespace;
            ContainingTypes = eventDispatcherModel.DefinitionType.ContainingTypes.ToArray();
            Classes = new ClassesTM(this, eventDispatcherModel);
            Interfaces = new InterfacesTM(this);
            Structs = new StructsTM(this);
            Exceptions = new ExceptionsTM(this);
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
    }
}
