using System;

namespace Tycho.Identity.Structure
{
    internal sealed class InstanceIdentity : Identity, IEquatable<InstanceIdentity>
    {
        private InstanceIdentity(string value) : base(value) { }

        public static InstanceIdentity Create(Type definitionType, string? instanceSuffix = null)
        {
            if (definitionType == null)
            {
                throw new ArgumentNullException(nameof(definitionType));
            }

            if (instanceSuffix != null && string.IsNullOrWhiteSpace(instanceSuffix))
            {
                throw new ArgumentException("Instance suffix cannot be empty.", nameof(instanceSuffix));
            }

            string definitionId = TypeIdentifier.GetId(definitionType);
            return Parse(instanceSuffix == null ? definitionId : definitionId + ":" + instanceSuffix);
        }

        public static InstanceIdentity Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Instance identity cannot be empty.", nameof(value));
            }

            return new InstanceIdentity(value);
        }

        public bool Equals(InstanceIdentity? other) => this == other;
    }
}
