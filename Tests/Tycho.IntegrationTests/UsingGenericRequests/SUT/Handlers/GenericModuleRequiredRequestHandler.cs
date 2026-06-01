using Tycho.IntegrationTests.UsingGenericRequests.SUT.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingGenericRequests.SUT.Handlers;

internal class GenericModuleRequiredRequestHandler<T>
    : IRequestHandler<GenericModuleRequiredRequest<T>, GenericModuleRequiredRequest<T>.Response<T>>
{
    public Task<GenericModuleRequiredRequest<T>.Response<T>> HandleAsync(GenericModuleRequiredRequest<T> requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(new GenericModuleRequiredRequest<T>.Response<T>(requestData.Data));
    }
}
