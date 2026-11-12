using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Tycho.Persistence.EFCore.Database;

internal sealed class ProviderDispatcher<TDelegate>(
    TDelegate fallback,
    IReadOnlyDictionary<string, TDelegate> overrides)
    where TDelegate : Delegate
{
    public TDelegate Resolve(DbContext dbContext) =>
        dbContext.Database.ProviderName is { } provider &&
        overrides.TryGetValue(provider, out TDelegate? implementation)
            ? implementation
            : fallback;
}
