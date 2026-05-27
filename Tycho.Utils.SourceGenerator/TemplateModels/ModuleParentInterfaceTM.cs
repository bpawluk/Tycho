using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Modules;
using Tycho.Utils.SourceGenerator.References.Tycho.Structure;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class ModuleParentInterfaceTM : TemplateModelBase
    {
        public string[] ContainingTypes { get; }

        public string[] OwnerConstraints { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public RequestTM[] Requests { get; }

        public ModuleParentInterfaceTM(TychoParentModel tychoParentModel)
        {
            Namespace = tychoParentModel.DefinitionType.Namespace;
            ContainingTypes = tychoParentModel.DefinitionType.ContainingTypeDeclarationSignatures.ToArray();
            OwnerConstraints = tychoParentModel.DefinitionType.TypeParameterConstraintClauses.ToArray();
            Classes = new ClassesTM(this, tychoParentModel);
            Interfaces = new InterfacesTM();
            Methods = new MethodsTM();
            Parameters = new ParametersTM();
            Requests = tychoParentModel.Requests.Select(r => new RequestTM(this, r)).ToArray();
        }

        internal class ClassesTM
        {
            public string ModuleClass { get; }
            public string ModuleBaseClass { get; }
            public string TaskClass { get; }
            public string CancellationTokenClass { get; }

            public ClassesTM(ModuleParentInterfaceTM owner, TychoParentModel tychoParentModel)
            {
                ModuleClass = tychoParentModel.DefinitionType.DeclarationName;
                ModuleBaseClass = owner.UseType(TychoModuleReference.TypeModel);
                TaskClass = owner.UseType(TaskReference.TypeModel);
                CancellationTokenClass = owner.UseType(CancellationTokenReference.TypeModel);
            }
        }

        internal class InterfacesTM
        {
            public string ParentInterface { get; }

            public InterfacesTM()
            {
                ParentInterface = ModuleParentSymbols.ParentInterface;
            }
        }

        internal class MethodsTM
        {
            public string ExecuteAsyncMethod { get; }

            public MethodsTM()
            {
                ExecuteAsyncMethod = ParentBaseReference.ExecuteAsyncMethodSignature.MethodName;
            }
        }

        internal class ParametersTM
        {
            public string RequestDataParameter { get; }
            public string CancellationTokenParameter { get; }

            public ParametersTM()
            {
                RequestDataParameter = ModuleParentSymbols.RequestDataParameter;
                CancellationTokenParameter = ModuleParentSymbols.CancellationTokenParameter;
            }
        }

        internal class RequestTM
        {
            public string RequestType { get; }
            public string ResponseType { get; }
            public bool HasResponse { get; }

            public RequestTM(ModuleParentInterfaceTM owner, TychoRequestModel tychoRequestModel)
            {
                RequestType = owner.UseType(tychoRequestModel.RequestType);
                HasResponse = tychoRequestModel.HasResponse;
                ResponseType = HasResponse ? owner.UseType(tychoRequestModel.ResponseType.Value) : string.Empty;
            }
        }
    }
}
