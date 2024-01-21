using IntegrationTests.ForwardingMessages.SUT.Submodules;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Interceptors;

namespace IntegrationTests.ForwardingMessages.SUT.Interceptors;

internal class QueryInterceptor :
    IQueryInterceptor<QueryToForward, string>,
    IQueryInterceptor<MappedAlphaQuery, AlphaResponse>,
    IQueryInterceptor<MappedBetaQuery, BetaResponse>,
    IQueryInterceptor<MappedQuery, string>
{
    public Task<string> ExecuteAfter(QueryToForward queryData, string result, CancellationToken cancellationToken = default)
    {
        queryData.PostInterceptions++;
        return Task.FromResult(result);
    }

    public Task<AlphaResponse> ExecuteAfter(MappedAlphaQuery queryData, AlphaResponse result, CancellationToken cancellationToken = default)
    {
        queryData.PostInterceptions++;
        return Task.FromResult(result);
    }

    public Task<BetaResponse> ExecuteAfter(MappedBetaQuery queryData, BetaResponse result, CancellationToken cancellationToken = default)
    {
        queryData.PostInterceptions++;
        return Task.FromResult(result);
    }

    public Task<string> ExecuteAfter(MappedQuery queryData, string result, CancellationToken cancellationToken = default)
    {
        queryData.PostInterceptions++;
        return Task.FromResult(result);
    }

    public Task ExecuteBefore(QueryToForward queryData, CancellationToken cancellationToken = default)
    {
        queryData.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(MappedAlphaQuery queryData, CancellationToken cancellationToken = default)
    {
        queryData.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(MappedBetaQuery queryData, CancellationToken cancellationToken = default)
    {
        queryData.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(MappedQuery queryData, CancellationToken cancellationToken = default)
    {
        queryData.PreInterceptions++;
        return Task.CompletedTask;
    }
}
