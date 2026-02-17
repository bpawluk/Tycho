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

        public StructsTM Structs { get; }

        public MethodsTM Methods { get; }

        public SymbolsTM Symbols { get; }

        public RequestTM[] Requests { get; }

        public ModuleFacadeTM(TychoFacadeModel tychoFacadeModel)
        {
            Namespace = tychoFacadeModel.DefinitionType.Namespace;
            ContainingTypes = tychoFacadeModel.DefinitionType.ContainingTypes.ToArray();
            Classes = new ClassesTM(this, tychoFacadeModel);
            Interfaces = new InterfacesTM(this, tychoFacadeModel);
            Structs = new StructsTM(this);
            Methods = new MethodsTM();
            Symbols = new SymbolsTM();
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

        internal class SymbolsTM
        {
            public string ExecuteAsyncMethod { get; }
            public string RequestDataParameter { get; }
            public string CancellationTokenParameter { get; }
            public string ModuleField { get; }
            public string ModuleParameter { get; }
            public string DisposeAsyncMethod { get; }

            public SymbolsTM()
            {
                ExecuteAsyncMethod = ModuleFacadeSymbols.ExecuteAsyncMethodName;
                RequestDataParameter = ModuleFacadeSymbols.RequestDataParameterName;
                CancellationTokenParameter = ModuleFacadeSymbols.CancellationTokenParameterName;
                ModuleField = ModuleFacadeSymbols.ModuleFieldName;
                ModuleParameter = ModuleFacadeSymbols.ModuleParameterName;
                DisposeAsyncMethod = ModuleFacadeSymbols.DisposeAsyncMethodName;
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
