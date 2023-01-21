﻿using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace Test.Integration.ContractDefinitionAndMessageHandling.SUT.ExternalHandlers;

internal class HandledByHandlerTypeCommandHandler : ICommandHandler<HandledByHandlerTypeCommand>
{
    private readonly ContractDefinitionAndMessageHandlingTests _tests;

    public HandledByHandlerTypeCommandHandler(ContractDefinitionAndMessageHandlingTests tests)
    {
        _tests = tests;
    }

    public Task Handle(HandledByHandlerTypeCommand commandData, CancellationToken cancellationToken)
    {
        _tests.CommandWorkflowCompleted = true;
        return Task.CompletedTask;
    }
}
