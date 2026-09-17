using System;
using System.Security.Cryptography;
using System.Text;
using Tycho.Structure;

namespace Tycho.Persistence.EFCore.Common;

internal sealed class PersistenceOwner
{
    public string Key { get; }

    public PersistenceOwner(Internals internals)
    {
        ArgumentNullException.ThrowIfNull(internals);
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(internals.OwnerInstanceId.Value));
        Key = Convert.ToHexString(hash.AsSpan(0, 16));
    }
}
