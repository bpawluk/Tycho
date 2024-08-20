using IntegrationTests.ContractDefinitionAndMessageHandling.SUT.InternalHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT;

// Events
public record HandledByLambdaEvent() : IEvent;
public record HandledByAsyncLambdaEvent() : IEvent;
public record HandledByHandlerInstanceEvent() : IEvent;
public record HandledByHandlerTypeEvent() : IEvent;

// Requests
public record HandledByLambdaRequest() : IRequest;
public record HandledByAsyncLambdaRequest() : IRequest;
public record HandledByHandlerInstanceRequest() : IRequest;
public record HandledByHandlerTypeRequest() : IRequest;

// Requests with responses
public record HandledByLambdaRequestWithResponse() : IRequest<string>;
public record HandledByAsyncLambdaRequestWithResponse() : IRequest<string>;
public record HandledByHandlerInstanceRequestWithResponse() : IRequest<string>;
public record HandledByHandlerTypeRequestWithResponse() : IRequest<string>;


internal class AppModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var thisModule = services.GetRequiredService<IModule>();
        module.Events.Handle<HandledByLambdaEvent>(e => thisModule.Publish(e))
              .Events.Handle<HandledByAsyncLambdaEvent>((e, t) =>
              {
                  thisModule.Publish(e, t);
                  return Task.CompletedTask;
              })
              .Events.Handle(new HandledByHandlerInstanceEventHandler(thisModule))
              .Events.Handle<HandledByHandlerTypeEvent, HandledByHandlerTypeEventHandler>();

        module.Requests.Handle<HandledByLambdaRequest>(c => { thisModule.Execute(c); })
              .Requests.Handle<HandledByAsyncLambdaRequest>((c, t) => thisModule.Execute(c, t))
              .Requests.Handle(new HandledByHandlerInstanceRequestHandler(thisModule))
              .Requests.Handle<HandledByHandlerTypeRequest, HandledByHandlerTypeRequestHandler>();

        module.Requests.Handle<HandledByLambdaRequestWithResponse, string>(q => thisModule.Execute<HandledByLambdaRequestWithResponse, string>(q).Result)
              .Requests.Handle<HandledByAsyncLambdaRequestWithResponse, string>(thisModule.Execute<HandledByAsyncLambdaRequestWithResponse, string>)
              .Requests.Handle(new HandledByHandlerInstanceRequestWithResponseHandler(thisModule))
              .Requests.Handle<HandledByHandlerTypeRequestWithResponse, string, HandledByHandlerTypeRequestWithResponseHandler>();
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Events.Declare<HandledByLambdaEvent>()
              .Events.Declare<HandledByAsyncLambdaEvent>()
              .Events.Declare<HandledByHandlerInstanceEvent>()
              .Events.Declare<HandledByHandlerTypeEvent>();

        module.Requests.Declare<HandledByLambdaRequest>()
              .Requests.Declare<HandledByAsyncLambdaRequest>()
              .Requests.Declare<HandledByHandlerInstanceRequest>()
              .Requests.Declare<HandledByHandlerTypeRequest>();

        module.Requests.Declare<HandledByLambdaRequestWithResponse, string>()
              .Requests.Declare<HandledByAsyncLambdaRequestWithResponse, string>()
              .Requests.Declare<HandledByHandlerInstanceRequestWithResponse, string>()
              .Requests.Declare<HandledByHandlerTypeRequestWithResponse, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(this);
    }
}
