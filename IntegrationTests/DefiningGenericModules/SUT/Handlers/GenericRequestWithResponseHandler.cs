using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.DefiningGenericModules.SUT.Handlers;

internal class GenericRequestWithResponseHandler<T> : IRequestHandler<GenericRequestWithResponse<T>, T>
{
    public Task<T> Handle(GenericRequestWithResponse<T> requestData, CancellationToken cancellationToken) => Task.FromResult(requestData.Data);
}
