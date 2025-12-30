using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Model;

namespace Tycho.Utils.SourceGenerator
{
    /// <summary>
    /// Augments classes marked with <c>ModuleDefinitionAttribute</c>
    /// </summary>
    [Generator]
    public class ModuleDefinitionSourceGenerator : TychoSourceGeneratorBase
    {
        protected override string AttributeName { get; } = "Tycho.Modules.ModuleDefinitionAttribute";

        protected override string EventsDefinitionMethodName { get; } = "DefineEvents";

        protected override string EventsDefinitionTypeName { get; } = "global::Tycho.Modules.IModuleEvents";

        protected override string EventHandlerDefinitionMethodName { get; } = "Handles";

        public override void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var pipeline = BuildPipeline(context);
            context.RegisterSourceOutput(pipeline, GenerateSources);
        }

        private void GenerateSources(SourceProductionContext context, TychoDefinitionModel model)
        {
            GenerateSource(context, model, "Templates/EventDispatcher.sbncs", $"{model.SourceNamespace}.{model.SourceClassName}EventDispatcher.g.cs");
            GenerateSource(context, model, "Templates/ModuleDefinition.sbncs", $"{model.SourceNamespace}.{model.SourceClassName}.setup.g.cs");
        }
    }
}
