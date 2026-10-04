using System;
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

        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(internals.OwnerId.Value));
        Identifier = Convert.ToHexString(hash.AsSpan(0, 16));

        if (logger?.IsEnabled(LogLevel.Information) == true)
        {
            logger.PersistenceOwnerConfigured(internals.OwnerId.Value, Identifier);
        }
    }
}
