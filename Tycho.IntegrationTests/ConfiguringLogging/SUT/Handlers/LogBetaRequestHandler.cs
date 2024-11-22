using Microsoft.Extensions.Logging;
using Tycho.IntegrationTests.ConfiguringLogging.SUT.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ConfiguringLogging.SUT.Handlers;

internal class LogBetaRequestHandler(ILogger<LogBetaRequestHandler> logger) : IRequestHandler<LogBetaRequest>
{
    private readonly ILogger<LogBetaRequestHandler> _logger = logger;

    public Task Handle(LogBetaRequest requestData, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Beta");
        return Task.CompletedTask;
    }
}