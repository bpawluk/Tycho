using System;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Tycho.Identity.Structure;

namespace Tycho.Structure
{
    internal sealed class AppInternals : Internals
    {
        public AppInternals(HostApplicationBuilder hostBuilder, Type ownerDefinition) : base(
            hostBuilder,
            new ControlPlane(),
            InstanceIdentity.Create(ownerDefinition))
        {
        }

        protected override void PrepareForStart(CancellationToken cancellationToken)
        {
            MaterializeModules(cancellationToken);
            ControlPlane.CompleteRegistration();
        }
    }
}
