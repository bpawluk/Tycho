using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Structure;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class AppInterfaceTM : TemplateModelBase
    {
        public string Namespace { get; }

        public string[] ContainingTypes { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public RequestTM[] Requests { get; }

        public AppInterfaceTM(TychoFacadeModel tychoFacadeModel)
        {
            Namespace = tychoFacadeModel.DefinitionType.Namespace;
            ContainingTypes = tychoFacadeModel.DefinitionType.ContainingTypes.ToArray();
            Classes = new ClassesTM(this);
            Interfaces = new InterfacesTM(this, tychoFacadeModel);
            Methods = new MethodsTM();
            Parameters = new ParametersTM();
            Requests = tychoFacadeModel.Requests.Select(r => new RequestTM(this, r)).ToArray();
        }

        internal class ClassesTM
        {
            public string TaskClass { get; }
            public string CancellationTokenClass { get; }

            public ClassesTM(AppInterfaceTM owner)
            {
                TaskClass = owner.UseType(TaskReference.TypeModel);
                CancellationTokenClass = owner.UseType(CancellationTokenReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string FacadeInterface { get; }
            public string AsyncDisposableInterface { get; }

            public InterfacesTM(AppInterfaceTM owner, TychoFacadeModel tychoFacadeModel)
            {
                FacadeInterface = AppFacadeSymbols.GetAppFacadeInterface(tychoFacadeModel.DefinitionType.Name);
                AsyncDisposableInterface = owner.UseType(IAsyncDisposableReference.TypeModel);
            }
        }

        internal class MethodsTM
        {
            public string ExecuteAsyncMethod { get; }

            public MethodsTM()
            {
                ExecuteAsyncMethod = IAppInstanceReference.ExecuteAsyncMethodSignature.MethodName;
            }
        }

        internal class ParametersTM
        {
            public string RequestDataParameter { get; }
            public string CancellationTokenParameter { get; }

            public ParametersTM()
            {
                RequestDataParameter = AppFacadeSymbols.RequestDataParameter;
                CancellationTokenParameter = AppFacadeSymbols.CancellationTokenParameter;
            }
        }

        internal class RequestTM
        {
            public string RequestType { get; }
            public string ResponseType { get; }
            public bool HasResponse { get; }

            public RequestTM(AppInterfaceTM owner, TychoRequestModel tychoRequestModel)
            {
                RequestType = owner.UseType(tychoRequestModel.RequestType);
                HasResponse = tychoRequestModel.HasResponse;
                ResponseType = HasResponse ? owner.UseType(tychoRequestModel.ResponseType.Value) : null;
            }
        }
    }
}
