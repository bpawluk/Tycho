using Microsoft.Extensions.Hosting;
using Tycho.Structure;
using Tycho.Identity.Structure;

namespace Tycho.Persistence.EFCore.UnitTests._Utils;

internal static class PersistenceTestInternals
{
    public static Internals Create(Type definitionType, Type? applicationType = null)
    {
        return new ModuleInternals(
            Host.CreateEmptyApplicationBuilder(null),
            new ControlPlane(InstanceIdentity.Create(applicationType ?? typeof(PersistenceTestInternals))),
            definitionType);
    }
}
