using System;
using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Tycho.Persistence.EFCore.Logging;
using Tycho.Structure;

namespace Tycho.Persistence.EFCore.Common;

internal sealed class PersistenceOwner
{
    public string Identifier { get; }

    public PersistenceOwner(Internals internals, ILogger<PersistenceOwner>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(internals);

        byte[] applicationId = Encoding.UTF8.GetBytes(internals.ControlPlane.ApplicationId.Value);
        byte[] ownerId = Encoding.UTF8.GetBytes(internals.OwnerId.Value);
        byte[] identity = new byte[8 + applicationId.Length + ownerId.Length];

        BinaryPrimitives.WriteInt32BigEndian(identity.AsSpan(0, 4), applicationId.Length);
        applicationId.CopyTo(identity, 4);

        int ownerOffset = 4 + applicationId.Length;
        BinaryPrimitives.WriteInt32BigEndian(identity.AsSpan(ownerOffset, 4), ownerId.Length);
        ownerId.CopyTo(identity, ownerOffset + 4);

        byte[] hash = SHA256.HashData(identity);
        Identifier = Convert.ToHexString(hash.AsSpan(0, 16));

        if (logger?.IsEnabled(LogLevel.Information) == true)
        {
            logger.PersistenceOwnerConfigured(internals.OwnerId.Value, Identifier);
        }
    }
}
