using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.References;
using Tycho.Utils.SourceGenerator.References.System;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class AppFacadeTM : TemplateModelBase
    {
        public string Namespace { get; }

        public string[] ContainingTypes { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public StructsTM Structs { get; }

        public RequestTM[] Requests { get; }

        public AppFacadeTM(TychoFacadeModel tychoFacadeModel)
        {
            Namespace = tychoFacadeModel.DefinitionType.Namespace;
            ContainingTypes = tychoFacadeModel.DefinitionType.ContainingTypes.ToArray();
            Classes = new ClassesTM(this, tychoFacadeModel);
            Interfaces = new InterfacesTM(this, tychoFacadeModel);
            Structs = new StructsTM(this);
            Requests = tychoFacadeModel.Requests.Select(r => new RequestTM(this, r)).ToArray();
        }

        internal class ClassesTM
        {
            public string AppClass { get; }
            public string FacadeClass { get; }
            public string TaskClass { get; }
            public string ValueTaskClass { get; }

            public ClassesTM(AppFacadeTM owner, TychoFacadeModel tychoFacadeModel)
            {
                AppClass = tychoFacadeModel.DefinitionType.Name;
                FacadeClass = $"{AppClass}Facade";
                TaskClass = owner.UseType(TaskReference.TypeModel);
                ValueTaskClass = owner.UseType(ValueTaskReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string AppInterface { get; }
            public string AsyncDisposableInterface { get; }
            public string InstanceInterface { get; }

            public InterfacesTM(AppFacadeTM owner, TychoFacadeModel tychoFacadeModel)
            {
                AppInterface = $"I{tychoFacadeModel.DefinitionType.Name}";
                AsyncDisposableInterface = owner.UseType(IAsyncDisposableReference.TypeModel);
                InstanceInterface = owner.UseType(IAppInstanceReference.TypeModel);
            }
        }

        internal class StructsTM
        {
            public string CancellationTokenStruct { get; }

            public StructsTM(AppFacadeTM owner)
            {
                CancellationTokenStruct = owner.UseType(CancellationTokenReference.TypeModel);
            }
        }

        internal class RequestTM
        {
            public string RequestType { get; }
            public string ResponseType { get; }
            public bool HasResponse { get; }

            public RequestTM(AppFacadeTM owner, TychoRequestModel tychoRequestModel)
            {
                RequestType = owner.UseType(tychoRequestModel.RequestType);
                HasResponse = tychoRequestModel.HasResponse;
                ResponseType = HasResponse ? owner.UseType(tychoRequestModel.ResponseType.Value) : null;
            }
        }
    }
}
