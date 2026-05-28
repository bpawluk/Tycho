using System.Linq;
using Tycho.Utils.SourceGenerator.Models;
using Tycho.Utils.SourceGenerator.Models.Tycho;
using Tycho.Utils.SourceGenerator.References.System;
using Tycho.Utils.SourceGenerator.References.Tycho.Structure;
using Tycho.Utils.SourceGenerator.Symbols;

namespace Tycho.Utils.SourceGenerator.TemplateModels
{
    internal class ModuleParentTM : TemplateModelBase
    {
        public string[] ContainingTypes { get; }

        public string[] OwnerConstraints { get; }

        public ClassesTM Classes { get; }

        public InterfacesTM Interfaces { get; }

        public MethodsTM Methods { get; }

        public ParametersTM Parameters { get; }

        public RequestTM[] Requests { get; }

        public ModuleParentTM(TychoParentModel tychoParentModel)
        {
            Namespace = tychoParentModel.DefinitionType.Namespace;
            ContainingTypes = UseContainingTypeDeclarations(tychoParentModel.DefinitionType);
            OwnerConstraints = UseConstraintClauses(tychoParentModel.DefinitionType.TypeParameters).ToArray();
            Classes = new ClassesTM(this, tychoParentModel);
            Interfaces = new InterfacesTM();
            Methods = new MethodsTM();
            Parameters = new ParametersTM();
            Requests = tychoParentModel.Requests.Select(r => new RequestTM(this, r)).ToArray();
        }

        internal class ClassesTM
        {
            public string ModuleClass { get; }
            public string ParentClass { get; }
            public string ParentClassWithTypeParams { get; }
            public string ParentBaseClass { get; }
            public string TaskClass { get; }
            public string CancellationTokenClass { get; }
            public string ParentReferenceClass { get; }

            public ClassesTM(ModuleParentTM owner, TychoParentModel tychoParentModel)
            {
                string moduleNameStem = tychoParentModel.DefinitionType.Name;
                ModuleClass = tychoParentModel.DefinitionType.DeclarationName;
                ParentClass = ModuleParentSymbols.GetParentClass(moduleNameStem);
                ParentClassWithTypeParams = ModuleParentSymbols.GetParentClass(moduleNameStem, tychoParentModel.DefinitionType.TypeParametersSuffix);
                ParentBaseClass = owner.UseType(ParentBaseReference.TypeModel);
                TaskClass = owner.UseType(TaskReference.TypeModel);
                CancellationTokenClass = owner.UseType(CancellationTokenReference.TypeModel);
                ParentReferenceClass = owner.UseType(IParentReferenceReference.TypeModel);
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
            public string ParentReferenceParameter { get; }
            public string RequestDataParameter { get; }
            public string CancellationTokenParameter { get; }

            public ParametersTM()
            {
                ParentReferenceParameter = ModuleParentSymbols.ParentReferenceParameter;
                RequestDataParameter = ModuleParentSymbols.RequestDataParameter;
                CancellationTokenParameter = ModuleParentSymbols.CancellationTokenParameter;
            }
        }

        internal class RequestTM
        {
            public string RequestType { get; }
            public string ResponseType { get; }
            public bool HasResponse { get; }

            public RequestTM(ModuleParentTM owner, TychoRequestModel tychoRequestModel)
            {
                RequestType = owner.UseType(tychoRequestModel.RequestType);
                HasResponse = tychoRequestModel.HasResponse;
                ResponseType = HasResponse ? owner.UseType(tychoRequestModel.ResponseType.Value) : string.Empty;
            }
        }
    }
}
