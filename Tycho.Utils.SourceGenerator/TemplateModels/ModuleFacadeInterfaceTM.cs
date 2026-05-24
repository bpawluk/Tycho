using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Modules;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class ModuleFacadeInterfaceTM : TemplateModelBase
    {
        public string Namespace { get; }

        public string[] ContainingTypes { get; }

        public string[] OwnerConstraints { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public RequestTM[] Requests { get; }

        public ModuleFacadeInterfaceTM(TychoFacadeModel tychoFacadeModel)
        {
            Namespace = tychoFacadeModel.DefinitionType.Namespace;
            ContainingTypes = tychoFacadeModel.DefinitionType.ContainingTypeDeclarationSignatures.ToArray();
            OwnerConstraints = tychoFacadeModel.DefinitionType.TypeParameterConstraintClauses.ToArray();
            Classes = new ClassesTM(this);
            Interfaces = new InterfacesTM(tychoFacadeModel);
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

            public InterfacesTM(TychoFacadeModel tychoFacadeModel)
            {
                ModuleInterface = $"{ModuleFacadeSymbols.GetModuleFacadeInterface(tychoFacadeModel.DefinitionType.Name)}{tychoFacadeModel.DefinitionType.TypeParametersSuffix}";
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
