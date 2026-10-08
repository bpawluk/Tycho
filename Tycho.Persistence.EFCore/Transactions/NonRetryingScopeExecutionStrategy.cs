using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Tycho.Persistence.EFCore.Transactions;

internal sealed class NonRetryingScopeExecutionStrategy(DbContext context)
    : ExecutionStrategy(context, maxRetryCount: 0, maxRetryDelay: TimeSpan.Zero)
{
    protected override bool ShouldRetryOn(Exception exception) => false;
}
