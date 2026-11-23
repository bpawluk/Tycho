using System;

namespace Tycho.Persistence.EFCore.Entries;

internal readonly record struct ClaimRequest(
    Guid ClaimId,
    DateTime UtcNow,
    DateTime Expiration,
    uint MaxAttempts);
