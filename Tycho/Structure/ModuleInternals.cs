using System;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Tycho.Identity.Structure;

namespace Tycho.Structure
{
    internal sealed class ModuleInternals : Internals
    {
        public ModuleInternals(
            HostApplicationBuilder hostBuilder,
            ControlPlane controlPlane,
            Type ownerDefinition,
            string? instanceSuffix = null)
            : base(
                  hostBuilder,
                  controlPlane,
                  InstanceIdentity.Create(ownerDefinition, instanceSuffix))
        {
        }

        protected override void PrepareForStart(CancellationToken cancellationToken)
        {
            ControlPlane.EnsureRegistrationComplete();
        }
    }
}
