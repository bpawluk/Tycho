using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.References;
using Tycho.Utils.SourceGenerator.References.System;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class ModuleFacadeTM : TemplateModelBase
    {
        public string Namespace { get; }

        public string[] ContainingTypes { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public StructsTM Structs { get; }

        public RequestTM[] Requests { get; }

        public ModuleFacadeTM(TychoFacadeModel tychoFacadeModel)
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
            public string ModuleClass { get; }
            public string FacadeClass { get; }
            public string TaskClass { get; }
            public string ValueTaskClass { get; }

            public ClassesTM(ModuleFacadeTM owner, TychoFacadeModel tychoFacadeModel)
            {
                ModuleClass = tychoFacadeModel.DefinitionType.Name;
                FacadeClass = $"{ModuleClass}Facade";
                TaskClass = owner.UseType(TaskReference.TypeModel);
                ValueTaskClass = owner.UseType(ValueTaskReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string ModuleInterface { get; }
            public string AsyncDisposableInterface { get; }
            public string InstanceInterface { get; }

            public InterfacesTM(ModuleFacadeTM owner, TychoFacadeModel tychoFacadeModel)
            {
                ModuleInterface = $"I{tychoFacadeModel.DefinitionType.Name}";
                AsyncDisposableInterface = owner.UseType(IAsyncDisposableReference.TypeModel);
                InstanceInterface = owner.UseType(IModuleInstanceReference.TypeModel);
            }
        }

        internal class StructsTM
        {
            public string CancellationTokenStruct { get; }

            public StructsTM(ModuleFacadeTM owner)
            {
                CancellationTokenStruct = owner.UseType(CancellationTokenReference.TypeModel);
            }
        }

        internal class RequestTM
        {
            public string RequestType { get; }
            public string ResponseType { get; }
            public bool HasResponse { get; }

            public RequestTM(ModuleFacadeTM owner, TychoRequestModel tychoRequestModel)
            {
                RequestType = owner.UseType(tychoRequestModel.RequestType);
                HasResponse = tychoRequestModel.HasResponse;
                ResponseType = HasResponse ? owner.UseType(tychoRequestModel.ResponseType.Value) : string.Empty;
            }
        }
    }
}
