using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Contracts;

internal class OutboxBuilder : IOutboxDefiner, IOutboxConsumer, IOutboxBuilder, IDisposable
{
    private readonly IMessageRouter _moduleInbox;
    private readonly ReaderWriterLockSlim _lock;
    private readonly HashSet<Type> _definedMessages;

    public OutboxBuilder(IMessageRouter moduleInbox)
    {
        _moduleInbox = moduleInbox;
        _lock = new ReaderWriterLockSlim();
        _definedMessages = new HashSet<Type>();
    }

    #region IOutboxDefiner
    public IOutboxDefiner Publishes<Event>() where Event 
        : class, IEvent
    {
        AddMessageDefinition(typeof(Event), nameof(Event));
        return this;
    }

    public IOutboxDefiner Requires<Command>() where Command
        : class, ICommand
    {
        AddMessageDefinition(typeof(Command), nameof(Command));
        return this;
    }

    public IOutboxDefiner Requires<Query, Response>()
        where Query : class, IQuery<Response>
    {
        AddMessageDefinition(typeof(Query), nameof(Query));
        return this;
    }
    #endregion

    #region IOutboxConsumer.Events
    public IOutboxConsumer OnEvent<Event>(Action<Event> action)
        where Event : class, IEvent
    {
        ValidateIfMessageIsDefined(typeof(Event), nameof(Event));
        var handler = new LambdaWrappingEventHandler<Event>(action);
        _moduleInbox.RegisterEventHandler(handler);
        return this;
    }

    public IOutboxConsumer OnEvent<Event>(Func<Event, Task> function)
        where Event : class, IEvent
    {
        ValidateIfMessageIsDefined(typeof(Event), nameof(Event));
        var handler = new LambdaWrappingEventHandler<Event>(function);
        _moduleInbox.RegisterEventHandler(handler);
        return this;
    }

    public IOutboxConsumer OnEvent<Event>(Func<Event, CancellationToken, Task> function)
        where Event : class, IEvent
    {
        ValidateIfMessageIsDefined(typeof(Event), nameof(Event));
        var handler = new LambdaWrappingEventHandler<Event>(function);
        _moduleInbox.RegisterEventHandler(handler);
        return this;
    }

    public IOutboxConsumer OnEvent<Event>(IEventHandler<Event> handler)
        where Event : class, IEvent
    {
        ValidateIfMessageIsDefined(typeof(Event), nameof(Event));
        _moduleInbox.RegisterEventHandler(handler);
        return this;
    }

    public IOutboxConsumer OnEvent<Event>(Func<IEventHandler<Event>> handlerCreator)
        where Event : class, IEvent
    {
        ValidateIfMessageIsDefined(typeof(Event), nameof(Event));
        var handler = new TransientEventHandler<Event>(handlerCreator);
        _moduleInbox.RegisterEventHandler(handler);
        return this;
    }
    #endregion

    #region IOutboxConsumer.Commands
    public IOutboxConsumer OnCommand<Command>(Action<Command> action)
        where Command : class, ICommand
    {
        ValidateIfMessageIsDefined(typeof(Command), nameof(Command));
        var handler = new LambdaWrappingCommandHandler<Command>(action);
        _moduleInbox.RegisterCommandHandler(handler);
        return this;
    }

    public IOutboxConsumer OnCommand<Command>(Func<Command, Task> function)
        where Command : class, ICommand
    {
        ValidateIfMessageIsDefined(typeof(Command), nameof(Command));
        var handler = new LambdaWrappingCommandHandler<Command>(function);
        _moduleInbox.RegisterCommandHandler(handler);
        return this;
    }

    public IOutboxConsumer OnCommand<Command>(Func<Command, CancellationToken, Task> function)
        where Command : class, ICommand
    {
        ValidateIfMessageIsDefined(typeof(Command), nameof(Command));
        var handler = new LambdaWrappingCommandHandler<Command>(function);
        _moduleInbox.RegisterCommandHandler(handler);
        return this;
    }

    public IOutboxConsumer OnCommand<Command>(ICommandHandler<Command> handler)
        where Command : class, ICommand
    {
        ValidateIfMessageIsDefined(typeof(Command), nameof(Command));
        _moduleInbox.RegisterCommandHandler(handler);
        return this;
    }

    public IOutboxConsumer OnCommand<Command>(Func<ICommandHandler<Command>> handlerCreator)
        where Command : class, ICommand
    {
        ValidateIfMessageIsDefined(typeof(Command), nameof(Command));
        var handler = new TransientCommandHandler<Command>(handlerCreator);
        _moduleInbox.RegisterCommandHandler(handler);
        return this;
    }

    public IOutboxConsumer Ignore<Command>()
        where Command : class, ICommand
    {
        ValidateIfMessageIsDefined(typeof(Command), nameof(Command));
        var handler = new StubCommandHandler<Command>();
        _moduleInbox.RegisterCommandHandler(handler);
        return this;
    }
    #endregion

    #region IOutboxConsumer.Queries
    public IOutboxConsumer OnQuery<Query, Response>(Func<Query, Response> function)
        where Query : class, IQuery<Response>
    {
        ValidateIfMessageIsDefined(typeof(Query), nameof(Query));
        var handler = new LambdaWrappingQueryHandler<Query, Response>(function);
        _moduleInbox.RegisterQueryHandler(handler);
        return this;
    }

    public IOutboxConsumer OnQuery<Query, Response>(Func<Query, Task<Response>> function)
        where Query : class, IQuery<Response>
    {
        ValidateIfMessageIsDefined(typeof(Query), nameof(Query));
        var handler = new LambdaWrappingQueryHandler<Query, Response>(function);
        _moduleInbox.RegisterQueryHandler(handler);
        return this;
    }

    public IOutboxConsumer OnQuery<Query, Response>(Func<Query, CancellationToken, Task<Response>> function)
        where Query : class, IQuery<Response>
    {
        ValidateIfMessageIsDefined(typeof(Query), nameof(Query));
        var handler = new LambdaWrappingQueryHandler<Query, Response>(function);
        _moduleInbox.RegisterQueryHandler(handler);
        return this;
    }

    public IOutboxConsumer OnQuery<Query, Response>(IQueryHandler<Query, Response> handler)
        where Query : class, IQuery<Response>
    {
        ValidateIfMessageIsDefined(typeof(Query), nameof(Query));
        _moduleInbox.RegisterQueryHandler(handler);
        return this;
    }

    public IOutboxConsumer OnQuery<Query, Response>(Func<IQueryHandler<Query, Response>> handlerCreator)
        where Query : class, IQuery<Response>
    {
        ValidateIfMessageIsDefined(typeof(Query), nameof(Query));
        var handler = new TransientQueryHandler<Query, Response>(handlerCreator);
        _moduleInbox.RegisterQueryHandler(handler);
        return this;
    }

    public IOutboxConsumer Return<Query, Response>(Response response)
        where Query : class, IQuery<Response>
    {
        ValidateIfMessageIsDefined(typeof(Query), nameof(Query));
        var handler = new StubQueryHandler<Query, Response>(response);
        _moduleInbox.RegisterQueryHandler(handler);
        return this;
    }
    #endregion

    public IMessageBroker Build()
    {
        // TODO: Make sure all defined commands and queries have handlers
        return new MessageBroker(_moduleInbox);
    }

    public void Dispose()
    {
        _lock.Dispose();
    }

    private void ValidateIfMessageIsDefined(Type messageType, string messageKind)
    {
        if (!IsMessageDefined(messageType))
        {
            throw new InvalidOperationException($"Could not register the handler because the {messageType.Name} {messageKind} is not defined for this module");
        }
    }

    private bool IsMessageDefined(Type messageType)
    {
        _lock.EnterReadLock();
        try
        {
            return _definedMessages.Contains(messageType);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    private void AddMessageDefinition(Type messageType, string messageKind)
    {
        var added = false;

        _lock.EnterWriteLock();
        try
        {
            added = _definedMessages.Add(messageType);
        }
        finally
        {
            _lock.ExitWriteLock();
        }

        if (!added)
        {
            throw new ArgumentException($"The {messageType.Name} {messageKind} is already defined for this module", nameof(messageType));
        }
    }
}
