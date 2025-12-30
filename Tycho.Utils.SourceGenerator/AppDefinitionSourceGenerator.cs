using Microsoft.CodeAnalysis;
using Tycho.Utils.SourceGenerator.Model;

namespace Tycho.Utils.SourceGenerator
{
    /// <summary>
    /// Augments classes marked with <c>AppDefinitionAttribute</c>
    /// </summary>
    [Generator]
    public class AppDefinitionSourceGenerator : TychoSourceGeneratorBase
    {
        protected override string AttributeName { get; } = "Tycho.Apps.AppDefinitionAttribute";

        protected override string EventsDefinitionMethodName { get; } = "DefineEvents";

        protected override string EventsDefinitionTypeName { get; } = "global::Tycho.Apps.IAppEvents";

        protected override string EventHandlerDefinitionMethodName { get; } = "Handles";

        public override void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var pipeline = BuildPipeline(context);
            context.RegisterSourceOutput(pipeline, GenerateSources);
        }

        private void GenerateSources(SourceProductionContext context, TychoDefinitionModel model)
        {
            GenerateSource(context, model, "Templates/EventDispatcher.sbncs", $"{model.SourceNamespace}.{model.SourceClassName}EventDispatcher.g.cs");
            GenerateSource(context, model, "Templates/AppDefinition.sbncs", $"{model.SourceNamespace}.{model.SourceClassName}.setup.g.cs");
        }
    }
}
