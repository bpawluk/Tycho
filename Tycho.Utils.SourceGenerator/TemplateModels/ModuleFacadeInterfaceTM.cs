using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Modules;
using Tycho.Utils.SourceGenerator.References.Tycho.Structure;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class ModuleFacadeInterfaceTM : TemplateModelBase
    {
        public ContainingTypeTM[] ContainingTypes { get; }

        public string[] OwnerConstraints { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public RequestTM[] Requests { get; }

        public ModuleFacadeInterfaceTM(TychoFacadeModel tychoFacadeModel)
        {
            Namespace = tychoFacadeModel.DefinitionType.Namespace;
            ContainingTypes = UseContainingTypes(tychoFacadeModel.DefinitionType.ContainingTypes);
            OwnerConstraints = UseConstraintClauses(tychoFacadeModel.DefinitionType.TypeParameters).ToArray();
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

            public ClassesTM(ModuleFacadeInterfaceTM owner)
            {
                TaskClass = owner.UseType(TaskReference.TypeModel);
                CancellationTokenClass = owner.UseType(CancellationTokenReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string ModuleInterface { get; }
            public string RunnableInterface { get; }
            public string DisposableInterface { get; }

            public InterfacesTM(ModuleFacadeInterfaceTM owner, TychoFacadeModel tychoFacadeModel)
            {
                ModuleInterface = ModuleFacadeSymbols.GetModuleFacadeInterface(tychoFacadeModel.DefinitionType.Name, tychoFacadeModel.DefinitionType.TypeParametersSuffix);
                RunnableInterface = owner.UseType(IRunnableReference.TypeModel);
                DisposableInterface = owner.UseType(IDisposableReference.TypeModel);
            }
        }

        internal class MethodsTM
        {
            public string ExecuteAsyncMethod { get; }

            public MethodsTM()
            {
                ExecuteAsyncMethod = ModuleFacadeBaseReference.ExecuteAsyncMethodSignature.MethodName;
            }
        }

        internal class ParametersTM
        {
            public string RequestDataParameter { get; }
            public string CancellationTokenParameter { get; }

            public ParametersTM()
            {
                RequestDataParameter = ModuleFacadeSymbols.RequestDataParameter;
                CancellationTokenParameter = ModuleFacadeSymbols.CancellationTokenParameter;
            }
        }

        internal class RequestTM
        {
            public string RequestType { get; }
            public string ResponseType { get; }
            public bool HasResponse { get; }

            public RequestTM(ModuleFacadeInterfaceTM owner, TychoRequestModel tychoRequestModel)
            {
                RequestType = owner.UseType(tychoRequestModel.RequestType);
                HasResponse = tychoRequestModel.HasResponse;
                ResponseType = HasResponse ? owner.UseType(tychoRequestModel.ResponseType.Value) : string.Empty;
            }
        }
    }
}
