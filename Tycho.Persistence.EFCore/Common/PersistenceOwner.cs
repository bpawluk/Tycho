using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Tycho.Structure;

namespace Tycho.Persistence.EFCore.Common;

internal sealed class PersistenceOwner
{
    public string Key { get; }

    public PersistenceOwner(Internals internals, ILogger<PersistenceOwner>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(internals);

        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(internals.OwnerInstanceId.Value));
        Key = Convert.ToHexString(hash.AsSpan(0, 16));

        if (logger?.IsEnabled(LogLevel.Information) == true)
        {
            logger.LogInformation("Persistence key {OwnerKey} configured for {OwnerInstanceId}", Key, internals.OwnerInstanceId.Value);
        }
    }
}
