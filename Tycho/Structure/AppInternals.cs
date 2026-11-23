using System;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Tycho.Identity.Structure;

namespace Tycho.Structure
{
    internal sealed class AppInternals : Internals
    {
        public AppInternals(HostApplicationBuilder hostBuilder, Type ownerDefinition)
            : this(hostBuilder, InstanceIdentity.Create(ownerDefinition))
        {
        }

        private AppInternals(HostApplicationBuilder hostBuilder, InstanceIdentity applicationId) : base(
            hostBuilder,
            new ControlPlane(applicationId),
            applicationId)
        {
        }

        protected override void PrepareForStart(CancellationToken cancellationToken)
        {
            MaterializeModules(cancellationToken);
            ControlPlane.CompleteRegistration();
        }
    }
}
