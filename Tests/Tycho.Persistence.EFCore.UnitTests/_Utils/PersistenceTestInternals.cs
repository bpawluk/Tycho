using Microsoft.Extensions.Hosting;
using Tycho.Identity.Structure;
using Tycho.Structure;

namespace Tycho.Persistence.EFCore.UnitTests._Utils;

internal static class PersistenceTestInternals
{
    public static Internals Create(Type definitionType, Type? applicationType = null, string? instanceSuffix = null)
    {
        return new ModuleInternals(
            Host.CreateEmptyApplicationBuilder(null),
            new ControlPlane(InstanceIdentity.Create(applicationType ?? typeof(PersistenceTestInternals))),
            definitionType,
            instanceSuffix);
    }
}
