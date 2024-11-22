using Microsoft.Extensions.Logging;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ConfiguringLogging.SUT.Handlers;

internal class LogAppRequestHandler(ILogger<LogAppRequestHandler> logger) : IRequestHandler<LogAppRequest>
{
    private readonly ILogger<LogAppRequestHandler> _logger = logger;

    public Task Handle(LogAppRequest requestData, CancellationToken cancellationToken)
    {
        _logger.LogInformation("App");
        return Task.CompletedTask;
    }
}