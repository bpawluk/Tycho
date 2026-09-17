using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.EdgeCaseTests.SUT;

[TychoDefinition]
public class UnrelatedClass { }

[TychoDefinition]
public abstract class AbstractApp : TychoApp { }

[TychoDefinition]
public abstract class AbstractModule : TychoModule { }

[TychoDefinition]
public class MissingContractApp : TychoApp
{
    protected override void DefineEvents(IAppEvents app) { }
    protected override void IncludeModules(IAppStructure app) { }
    protected override void RegisterServices(IServiceCollection app) { }
}

[TychoDefinition]
public class MissingEventsApp : TychoApp
{
    protected override void DefineContract(IAppContract app) { }
    protected override void IncludeModules(IAppStructure app) { }
    protected override void RegisterServices(IServiceCollection app) { }
}

[TychoDefinition]
public class MissingModulesApp : TychoApp
{
    protected override void DefineContract(IAppContract app) { }
    protected override void DefineEvents(IAppEvents app) { }
    protected override void RegisterServices(IServiceCollection app) { }
}

[TychoDefinition]
public class MissingContractModule : TychoModule
{
    protected override void DefineEvents(IModuleEvents module) { }
    protected override void IncludeModules(IModuleStructure module) { }
    protected override void RegisterServices(IServiceCollection module) { }
}

[TychoDefinition]
public class MissingEventsModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }
    protected override void IncludeModules(IModuleStructure module) { }
    protected override void RegisterServices(IServiceCollection module) { }
}

[TychoDefinition]
public class MissingModulesModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }
    protected override void DefineEvents(IModuleEvents module) { }
    protected override void RegisterServices(IServiceCollection module) { }
}

[TychoDefinition]
public class ValidApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        Expects<object>();
    }

    protected override void DefineEvents(IAppEvents app)
    {
        EventHelpers.Expects<object>();
        HandlesWith<object>();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        Uses<object>();
        Uses<UnrelatedModule>("unrelated overload");
    }

    protected override void RegisterServices(IServiceCollection app) { }

    private void DefineContract(string unrelated) { }

    private static IAppRequestBinding<T> Expects<T>() => throw new System.NotSupportedException();

    private static IAppEvents HandlesWith<T>() => throw new System.NotSupportedException();

    private static IAppStructure Uses<T>() => throw new System.NotSupportedException();

    private static IAppStructure Uses<TModule>(string unrelated) => throw new System.NotSupportedException();

    private sealed class UnrelatedModule { }
}

internal static class EventHelpers
{
    public static IAppEventBinding<T> Expects<T>() => throw new System.NotSupportedException();
}
