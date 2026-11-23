using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingGenericRequests.SUT.Handlers;

internal class GenericAppRequestHandler<T> : IRequestHandler<GenericAppRequest<T>, GenericAppRequest<T>.Response<T>>
{
    public Task<GenericAppRequest<T>.Response<T>> HandleAsync(GenericAppRequest<T> requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(new GenericAppRequest<T>.Response<T>(requestData.Data));
    }
}
