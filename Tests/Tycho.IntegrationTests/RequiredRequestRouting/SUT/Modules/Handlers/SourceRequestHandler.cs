using Tycho.Requests;

namespace Tycho.IntegrationTests.RequiredRequestRouting.SUT.Modules.Handlers;

internal sealed class SourceRequestHandler(ISourceModuleParent parent)
    : IRequestHandler<PlainCommand>, IRequestHandler<PlainQuery, string>,
      IRequestHandler<MappedCommand>, IRequestHandler<MappedQuery, string>,
      IRequestHandler<IgnoredCommand>, IRequestHandler<IgnoredQuery, string>
{
    public Task HandleAsync(PlainCommand request, CancellationToken token) => parent.ExecuteAsync(request, token);
    public Task<string> HandleAsync(PlainQuery request, CancellationToken token) => parent.ExecuteAsync(request, token);
    public Task HandleAsync(MappedCommand request, CancellationToken token) => parent.ExecuteAsync(request, token);
    public Task<string> HandleAsync(MappedQuery request, CancellationToken token) => parent.ExecuteAsync(request, token);
    public Task HandleAsync(IgnoredCommand request, CancellationToken token) => parent.ExecuteAsync(request, token);
    public Task<string> HandleAsync(IgnoredQuery request, CancellationToken token) => parent.ExecuteAsync(request, token);
}
