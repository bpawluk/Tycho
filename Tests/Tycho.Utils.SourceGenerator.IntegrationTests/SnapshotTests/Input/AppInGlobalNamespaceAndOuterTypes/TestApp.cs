using Tycho;
using Tycho.Apps;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable CA1050

public partial class AppOuter
{
    public partial class AppInner
    {

        [TychoDefinition]
        public class TestApp : TychoApp
        {
            protected override void DefineContract(IAppContract app) { }
            protected override void DefineEvents(IAppEvents app) { }
            protected override void IncludeModules(IAppStructure app) { }
            protected override void RegisterServices(IServiceCollection app) { }
        }
    }
}

#pragma warning disable CA1050
