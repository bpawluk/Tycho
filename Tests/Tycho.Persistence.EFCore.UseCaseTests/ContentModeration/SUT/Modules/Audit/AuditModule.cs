using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Audit.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Audit.Handlers;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Audit.Persistence;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Audit;

[TychoDefinition]
public partial class AuditModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Expects<RecordAuditRequest>()
              .HandlesWith<RecordAuditRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Expects<AuditRecordedEvent>()
              .Exposes();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddTychoPersistence<AuditDbContext>();
    }
}
