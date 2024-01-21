using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Forwarders;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Outbox.Builder
{
    internal partial class OutboxBuilder : IOutboxDefinition, IOutboxConsumer
    {
        public IOutboxDefinition Sends<Command>() where Command
            : class, ICommand
        {
            AddMessageDefinition(typeof(Command), nameof(Command));
            return this;
        }

        public IOutboxConsumer HandleCommand<Command>(Action<Command> action)
            where Command : class, ICommand
        {
            var handler = new LambdaWrappingCommandHandler<Command>(action);
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleCommand<Command>(Func<Command, Task> function)
            where Command : class, ICommand
        {
            var handler = new LambdaWrappingCommandHandler<Command>(function);
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleCommand<Command>(Func<Command, CancellationToken, Task> function)
            where Command : class, ICommand
        {
            var handler = new LambdaWrappingCommandHandler<Command>(function);
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleCommand<Command>(ICommandHandler<Command> handler)
            where Command : class, ICommand
        {
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleCommand<Command, Handler>()
            where Handler : class, ICommandHandler<Command>
            where Command : class, ICommand
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientCommandHandler<Command>(handlerCreator);
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer ForwardCommand<Command, Module>()
            where Command : class, ICommand
            where Module : TychoModule
        {
            Func<Command, Command> mapping = commandData => commandData;
            Func<ICommandHandler<Command>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<CommandDownForwarder<Command, Command, Module>>(mapping);
            };
            var handler = new TransientCommandHandler<Command>(handlerCreator);
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer ForwardCommand<Command, Interceptor, Module>()
            where Command : class, ICommand
            where Interceptor : class, ICommandInterceptor<Command>
            where Module : TychoModule
        {
            Func<Command, Command> mapping = commandData => commandData;
            Func<ICommandHandler<Command>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<CommandDownForwarder<Command, Command, Module>>(mapping, interceptor);
            };
            var handler = new TransientCommandHandler<Command>(handlerCreator);
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer ForwardCommand<CommandIn, CommandOut, Module>(Func<CommandIn, CommandOut> commandMapping)
            where CommandIn : class, ICommand
            where CommandOut : class, ICommand
            where Module : TychoModule
        {
            Func<ICommandHandler<CommandIn>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<CommandDownForwarder<CommandIn, CommandOut, Module>>(commandMapping);
            };
            var handler = new TransientCommandHandler<CommandIn>(handlerCreator);
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer ForwardCommand<CommandIn, CommandOut, Interceptor, Module>(Func<CommandIn, CommandOut> commandMapping)
            where CommandIn : class, ICommand
            where CommandOut : class, ICommand
            where Interceptor : class, ICommandInterceptor<CommandOut>
            where Module : TychoModule
        {
            Func<ICommandHandler<CommandIn>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<CommandDownForwarder<CommandIn, CommandOut, Module>>(commandMapping, interceptor);
            };
            var handler = new TransientCommandHandler<CommandIn>(handlerCreator);
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer ExposeCommand<Command>()
            where Command : class, ICommand
        {
            Func<Command, Command> mapping = commandData => commandData;
            Func<ICommandHandler<Command>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<CommandUpForwarder<Command, Command>>(mapping);
            };
            var handler = new TransientCommandHandler<Command>(handlerCreator);
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer ExposeCommand<Command, Interceptor>()
            where Command : class, ICommand
            where Interceptor : class, ICommandInterceptor<Command>
        {
            Func<Command, Command> mapping = commandData => commandData;
            Func<ICommandHandler<Command>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<CommandUpForwarder<Command, Command>>(mapping, interceptor);
            };
            var handler = new TransientCommandHandler<Command>(handlerCreator);
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer ExposeCommand<CommandIn, CommandOut>(Func<CommandIn, CommandOut> commandMapping)
            where CommandIn : class, ICommand
            where CommandOut : class, ICommand
        {
            Func<ICommandHandler<CommandIn>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<CommandUpForwarder<CommandIn, CommandOut>>(commandMapping);
            };
            var handler = new TransientCommandHandler<CommandIn>(handlerCreator);
            RegisterCommandHandler(handler);
            return this;

        }

        public IOutboxConsumer ExposeCommand<CommandIn, CommandOut, Interceptor>(Func<CommandIn, CommandOut> commandMapping)
            where CommandIn : class, ICommand
            where CommandOut : class, ICommand
            where Interceptor : class, ICommandInterceptor<CommandOut>
        {
            Func<ICommandHandler<CommandIn>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<CommandUpForwarder<CommandIn, CommandOut>>(commandMapping, interceptor);
            };
            var handler = new TransientCommandHandler<CommandIn>(handlerCreator);
            RegisterCommandHandler(handler);
            return this;

        }

        public IOutboxConsumer IgnoreCommand<Command>()
            where Command : class, ICommand
        {
            var handler = new StubCommandHandler<Command>();
            RegisterCommandHandler(handler);
            return this;
        }

        private void RegisterCommandHandler<Command>(ICommandHandler<Command> handler)
            where Command : class, ICommand
        {
            ValidateIfMessageIsDefined(typeof(Command), nameof(Command));
            _moduleInbox.RegisterCommandHandler(handler);
            MarkMessageAsHandled(typeof(Command));
        }
    }
}
