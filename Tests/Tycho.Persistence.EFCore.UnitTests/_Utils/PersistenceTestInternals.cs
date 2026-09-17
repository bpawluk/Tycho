using Microsoft.Extensions.Hosting;
using Tycho.Identity.Structure;
using Tycho.Persistence.EFCore.Common;
using Tycho.Structure;

namespace Tycho.Persistence.EFCore.UnitTests._Utils;

internal static class PersistenceTestInternals
{
    public static Internals Create(Type definitionType)
    {
        var parent = InstanceIdentity.CreateRoot(DefinitionIdentity.Create<PersistenceOwner>());
        return new Internals(Host.CreateEmptyApplicationBuilder(null), definitionType, parent);
    }
}
