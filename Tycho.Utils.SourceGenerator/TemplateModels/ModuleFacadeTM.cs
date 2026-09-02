using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Modules;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class ModuleFacadeTM : TemplateModelBase
    {
        public ContainingTypeTM[] ContainingTypes { get; }

        public string[] OwnerConstraints { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public RequestTM[] Requests { get; }

        public ModuleFacadeTM(TychoFacadeModel tychoFacadeModel)
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
            public string ModuleClass { get; }
            public string FacadeClass { get; }
            public string FacadeClassWithTypeParams { get; }
            public string FacadeBaseClass { get; }
            public string TaskClass { get; }
            public string ValueTaskClass { get; }
            public string CancellationTokenClass { get; }

            public ClassesTM(ModuleFacadeTM owner, TychoFacadeModel tychoFacadeModel)
            {
                string moduleNameStem = tychoFacadeModel.DefinitionType.Name;
                var facadeType = new GeneratedTypeModel(
                    tychoFacadeModel.DefinitionType,
                    ModuleFacadeSymbols.GetModuleFacadeClass(moduleNameStem));
                ModuleClass = tychoFacadeModel.DefinitionType.DeclarationName;
                FacadeClass = facadeType.Identifier;
                FacadeClassWithTypeParams = facadeType.DeclarationName;
                FacadeBaseClass = owner.UseType(ModuleFacadeBaseReference.TypeModel);
                TaskClass = owner.UseType(TaskReference.TypeModel);
                ValueTaskClass = owner.UseType(ValueTaskReference.TypeModel);
                CancellationTokenClass = owner.UseType(CancellationTokenReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string ModuleInterface { get; }
            public string InstanceInterface { get; }

            public InterfacesTM(ModuleFacadeTM owner, TychoFacadeModel tychoFacadeModel)
            {
                var facadeInterfaceType = new GeneratedTypeModel(
                    tychoFacadeModel.DefinitionType,
                    ModuleFacadeSymbols.GetModuleFacadeInterface(tychoFacadeModel.DefinitionType.Name));
                ModuleInterface = facadeInterfaceType.ReferenceName;
                InstanceInterface = owner.UseType(IModuleReference.TypeModel);
            }
        }

        internal class MethodsTM
        {
            public string ExecuteAsyncMethod { get; }
            public string ConfigureAwaitMethod { get; }

            public MethodsTM()
            {
                ExecuteAsyncMethod = ModuleFacadeBaseReference.ExecuteAsyncMethodSignature.MethodName;
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
