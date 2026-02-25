using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.References;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class ModuleFacadeTM : TemplateModelBase
    {
        public string Namespace { get; }

        public string[] ContainingTypes { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }


        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public RequestTM[] Requests { get; }

        public ModuleFacadeTM(TychoFacadeModel tychoFacadeModel)
        {
            Namespace = tychoFacadeModel.DefinitionType.Namespace;
            ContainingTypes = tychoFacadeModel.DefinitionType.ContainingTypes.ToArray();
            Classes = new ClassesTM(this, tychoFacadeModel);
            Interfaces = new InterfacesTM(this, tychoFacadeModel);
            Methods = new MethodsTM();
            Parameters = new ParametersTM();
            Requests = tychoFacadeModel.Requests.Select(r => new RequestTM(this, r)).ToArray();
        }

        internal class ClassesTM
        {
            public string ModuleClass { get; }
            public string FacadeClass { get; }
            public string TaskClass { get; }
            public string ValueTaskClass { get; }
            public string CancellationTokenClass { get; }

            public ClassesTM(ModuleFacadeTM owner, TychoFacadeModel tychoFacadeModel)
            {
                ModuleClass = tychoFacadeModel.DefinitionType.Name;
                FacadeClass = ModuleFacadeSymbols.GetModuleFacadeClass(ModuleClass);
                TaskClass = owner.UseType(TaskReference.TypeModel);
                ValueTaskClass = owner.UseType(ValueTaskReference.TypeModel);
                CancellationTokenClass = owner.UseType(CancellationTokenReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string ModuleInterface { get; }
            public string AsyncDisposableInterface { get; }
            public string InstanceInterface { get; }

            public InterfacesTM(ModuleFacadeTM owner, TychoFacadeModel tychoFacadeModel)
            {
                ModuleInterface = ModuleFacadeSymbols.GetModuleFacadeInterface(tychoFacadeModel.DefinitionType.Name);
                AsyncDisposableInterface = owner.UseType(IAsyncDisposableReference.TypeModel);
                InstanceInterface = owner.UseType(IModuleInstanceReference.TypeModel);
            }
        }

        internal class MethodsTM
        {
            public string ExecuteAsyncMethod { get; }
            public string DisposeAsyncMethod { get; }
            public string ConfigureAwaitMethod { get; }

            public MethodsTM()
            {
                ExecuteAsyncMethod = IModuleInstanceReference.ExecuteAsyncMethodSignature.MethodName;
                DisposeAsyncMethod = IAsyncDisposableReference.DisposeAsyncMethodSignature.MethodName;
                ConfigureAwaitMethod = ValueTaskReference.ConfigureAwaitMethodSignature.MethodName;
            }
        }

        internal class ParametersTM
        {
            public string ModuleParameter { get; }
            public string RequestDataParameter { get; }
            public string CancellationTokenParameter { get; }

            public ParametersTM()
            {
                RequestDataParameter = ModuleFacadeSymbols.RequestDataParameter;
                CancellationTokenParameter = ModuleFacadeSymbols.CancellationTokenParameter;
                ModuleParameter = ModuleFacadeSymbols.ModuleParameter;
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
