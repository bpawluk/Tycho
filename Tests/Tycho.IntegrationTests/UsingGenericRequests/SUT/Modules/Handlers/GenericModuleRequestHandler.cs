using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingGenericRequests.SUT.Modules.Handlers;

internal class GenericModuleRequestHandler<T>(ITestModuleParent parent) : IRequestHandler<GenericModuleRequest<T>, GenericModuleRequest<T>.Response<T>>
{
    public async Task<GenericModuleRequest<T>.Response<T>> HandleAsync(GenericModuleRequest<T> requestData, CancellationToken cancellationToken)
    {
        if (requestData is GenericModuleRequest<string> stringRequest)
        {
            GenericModuleRequiredRequest<string>.Response<string> result = await parent.ExecuteAsync(new GenericModuleRequiredRequest<string>(stringRequest.Data), cancellationToken);
            return new GenericModuleRequest<T>.Response<T>((T)(object)result.Data);
        }

        if (requestData is GenericModuleRequest<int> intRequest)
        {
            GenericModuleRequiredRequest<int>.Response<int> result = await parent.ExecuteAsync(new GenericModuleRequiredRequest<int>(intRequest.Data), cancellationToken);
            return new GenericModuleRequest<T>.Response<T>((T)(object)result.Data);
        }

        throw new InvalidOperationException($"Unsupported request type: {typeof(T)}");
    }
}
