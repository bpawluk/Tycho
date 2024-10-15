﻿namespace Tycho.IntegrationTests._Utils;

public class TestWorkflow<TResult>
{
    private readonly TaskCompletionSource<TResult> _resultCompletionSource = new();

    public Task<TResult> GetResult()
    {
        return _resultCompletionSource.Task;
    }

    public void SetResult(TResult result)
    {
        _resultCompletionSource.SetResult(result);
    }
}