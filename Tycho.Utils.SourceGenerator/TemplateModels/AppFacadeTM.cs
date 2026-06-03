using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Apps;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class AppFacadeTM : TemplateModelBase
    {
        public ContainingTypeTM[] ContainingTypes { get; }

        public string[] OwnerConstraints { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public RequestTM[] Requests { get; }

        public AppFacadeTM(TychoFacadeModel tychoFacadeModel)
        {
            Namespace = tychoFacadeModel.DefinitionType.Namespace;
            ContainingTypes = UseContainingTypes(tychoFacadeModel.DefinitionType.ContainingTypes);
            OwnerConstraints = UseConstraintClauses(tychoFacadeModel.DefinitionType.TypeParameters).ToArray();
            Classes = new ClassesTM(this, tychoFacadeModel);
            Interfaces = new InterfacesTM(this, tychoFacadeModel);
            Methods = new MethodsTM();
            Parameters = new ParametersTM();
            Requests = tychoFacadeModel.Requests.Select(r => new RequestTM(this, r)).ToArray();
        }

        internal class ClassesTM
        {
            public string AppClass { get; }
            public string FacadeClass { get; }
            public string FacadeClassWithTypeParams { get; }
            public string FacadeBaseClass { get; }
            public string TaskClass { get; }
            public string ValueTaskClass { get; }
            public string CancellationTokenClass { get; }

            public ClassesTM(AppFacadeTM owner, TychoFacadeModel tychoFacadeModel)
            {
                AppClass = tychoFacadeModel.DefinitionType.DeclarationName;
                FacadeClass = AppFacadeSymbols.GetAppFacadeClass(tychoFacadeModel.DefinitionType.Name);
                FacadeClassWithTypeParams = AppFacadeSymbols.GetAppFacadeClass(tychoFacadeModel.DefinitionType.Name, tychoFacadeModel.DefinitionType.TypeParametersSuffix);
                FacadeBaseClass = owner.UseType(AppFacadeBaseReference.TypeModel);
                TaskClass = owner.UseType(TaskReference.TypeModel);
                ValueTaskClass = owner.UseType(ValueTaskReference.TypeModel);
                CancellationTokenClass = owner.UseType(CancellationTokenReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string FacadeInterface { get; }
            public string InstanceInterface { get; }

            public InterfacesTM(AppFacadeTM owner, TychoFacadeModel tychoFacadeModel)
            {
                FacadeInterface = AppFacadeSymbols.GetAppFacadeInterface(tychoFacadeModel.DefinitionType.Name, tychoFacadeModel.DefinitionType.TypeParametersSuffix);
                InstanceInterface = owner.UseType(IAppReference.TypeModel);
            }
        }

        internal class MethodsTM
        {
            public string ExecuteAsyncMethod { get; }
            public string ConfigureAwaitMethod { get; }

            public MethodsTM()
            {
                ExecuteAsyncMethod = AppFacadeBaseReference.ExecuteAsyncMethodSignature.MethodName;
                ConfigureAwaitMethod = ValueTaskReference.ConfigureAwaitMethodSignature.MethodName;
            }
        }

        internal class ParametersTM
        {
            public string AppParameter { get; }
            public string RequestDataParameter { get; }
            public string CancellationTokenParameter { get; }

            public ParametersTM()
            {
                AppParameter = AppFacadeSymbols.AppParameter;
                RequestDataParameter = AppFacadeSymbols.RequestDataParameter;
                CancellationTokenParameter = AppFacadeSymbols.CancellationTokenParameter;
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
