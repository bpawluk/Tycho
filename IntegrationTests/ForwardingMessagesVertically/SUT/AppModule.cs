using IntegrationTests.ForwardingMessagesVertically.SUT.Submodules.Alpha;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Structure;

namespace IntegrationTests.ForwardingMessagesVertically.SUT;

internal class AppModule : TychoModule
{
    protected override void DeclareOutgoingMessages(IOutboxDefinition outbox, IServiceProvider services)
    {
        outbox.Events.Declare<EventToForward>()
              .Events.Declare<MappedEvent>();

        outbox.Requests.Declare<RequestToForward>()
              .Requests.Declare<MappedRequest>();

        outbox.Requests.Declare<RequestWithResponseToForward, string>()
              .Requests.Declare<MappedRequestWithResponse, string>();
    }

    protected override void HandleIncomingMessages(IInboxDefinition inbox, IServiceProvider services)
    {
        inbox.Events.Forward<EventToForward, AlphaModule>()
             .Events.Forward<EventToForwardWithMapping, AlphaInEvent, AlphaModule>(
                eventData => new(eventData.Result));

        inbox.Requests.Forward<RequestToForward, AlphaModule>()
             .Requests.Forward<RequestToForwardWithMapping, AlphaInRequest, AlphaModule>(
                requestData => new(requestData.Result));

        inbox.Requests.Forward<RequestWithResponseToForward, string, AlphaModule>()
             .Requests.Forward<
                RequestWithResponseToForwardWithMapping, string, 
                AlphaInRequestWithResponse, AlphaInRequestWithResponse.Response, 
                AlphaModule>(
                    requestData => new(requestData.Result),
                    response => response.Value);
    }

    protected override void IncludeSubmodules(ISubstructureDefinition structure, IServiceProvider services)
    {
        structure.AddSubmodule<AlphaModule>(alphaOutbox =>
        {
            alphaOutbox.Events.Expose<EventToForward>()
                       .Events.Expose<AlphaOutEvent, MappedEvent>(eventData => new(eventData.Result));

            alphaOutbox.Requests.Expose<RequestToForward>()
                       .Requests.Expose<AlphaOutRequest, MappedRequest>(requestData => new(requestData.Result));

            alphaOutbox.Requests.Expose<RequestWithResponseToForward, string>()
                       .Requests.Expose<AlphaOutRequestWithResponse, AlphaOutRequestWithResponse.Response, MappedRequestWithResponse, string>(
                            requestData => new(requestData.Result),
                            response => new(response));
        });
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
