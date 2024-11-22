using Microsoft.Extensions.Logging;
using Tycho.IntegrationTests.ConfiguringLogging.SUT.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ConfiguringLogging.SUT.Handlers;

internal class LogAlphaRequestHandler(ILogger<LogAlphaRequestHandler> logger) : IRequestHandler<LogAlphaRequest>
{
    private readonly ILogger<LogAlphaRequestHandler> _logger = logger;

    public Task Handle(LogAlphaRequest requestData, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Alpha");
        return Task.CompletedTask;
    }
}