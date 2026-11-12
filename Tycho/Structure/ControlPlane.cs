using System;
using System.Collections.Generic;
using System.Threading;
using Tycho.Identity.Structure;

namespace Tycho.Structure
{
    internal sealed class ControlPlane
    {
        private readonly Dictionary<InstanceIdentity, ModuleReference> _modules = new();
        private bool _registrationComplete;

        public InstanceIdentity ApplicationId { get; }

        public ControlPlane(InstanceIdentity applicationId)
        {
            ApplicationId = applicationId ?? throw new ArgumentNullException(nameof(applicationId));
        }

        public void RegisterModule(InstanceIdentity id, ModuleReference module)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            if (Volatile.Read(ref _registrationComplete))
            {
                throw new InvalidOperationException("Control plane registration has already completed.");
            }

            if (_modules.ContainsKey(id))
            {
                throw new InvalidOperationException($"Module instance with ID '{id}' has already been registered.");
            }

            _modules.Add(id, module);
        }

        public ModuleReference GetModule(InstanceIdentity id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            EnsureRegistrationComplete();

            if (_modules.TryGetValue(id, out ModuleReference? module))
            {
                return module;
            }

            throw new InvalidOperationException($"Module instance '{id}' is not registered.");
        }

        public void CompleteRegistration()
        {
            Volatile.Write(ref _registrationComplete, true);
        }

        public void EnsureRegistrationComplete()
        {
            if (!Volatile.Read(ref _registrationComplete))
            {
                throw new InvalidOperationException("Control plane registration is incomplete. Start the application before starting modules or delivering events.");
            }
        }
    }
}
