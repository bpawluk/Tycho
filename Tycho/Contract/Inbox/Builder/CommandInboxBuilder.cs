using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Forwarders;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Inbox.Builder
{
    internal partial class InboxBuilder : IInboxDefinition
    {
        public IInboxDefinition Executes<Command>(Action<Command> action)
            where Command : class, ICommand
        {
            var handler = new LambdaWrappingCommandHandler<Command>(action);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        public IInboxDefinition Executes<Command>(Func<Command, Task> function)
            where Command : class, ICommand
        {
            var handler = new LambdaWrappingCommandHandler<Command>(function);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        public IInboxDefinition Executes<Command>(Func<Command, CancellationToken, Task> function)
            where Command : class, ICommand
        {
            var handler = new LambdaWrappingCommandHandler<Command>(function);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        public IInboxDefinition Executes<Command>(ICommandHandler<Command> handler)
            where Command : class, ICommand
        {
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        public IInboxDefinition Executes<Command, Handler>()
            where Handler : class, ICommandHandler<Command>
            where Command : class, ICommand
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientCommandHandler<Command>(handlerCreator);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        public IInboxDefinition ForwardsCommand<Command, Module>()
            where Command : class, ICommand
            where Module : TychoModule
        {
            Func<Command, Command> mapping = commandData => commandData;
            Func<CommandDownForwarder<Command, Command, Module>> forwarderCreator = () =>
            {
                return _instanceCreator.CreateInstance<CommandDownForwarder<Command, Command, Module>>(mapping);
            };
            var handler = new TransientCommandHandler<Command>(forwarderCreator);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        public IInboxDefinition ForwardsCommand<Command, Interceptor, Module>()
            where Command : class, ICommand
            where Interceptor : class, ICommandInterceptor<Command>
            where Module : TychoModule
        {
            Func<Command, Command> mapping = commandData => commandData;
            Func<CommandDownForwarder<Command, Command, Module>> forwarderCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<CommandDownForwarder<Command, Command, Module>>(mapping, interceptor);
            };
            var handler = new TransientCommandHandler<Command>(forwarderCreator);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        public IInboxDefinition ForwardsCommand<CommandIn, CommandOut, Module>(Func<CommandIn, CommandOut> commandMapping)
            where CommandIn : class, ICommand
            where CommandOut : class, ICommand
            where Module : TychoModule
        {
            Func<CommandDownForwarder<CommandIn, CommandOut, Module>> forwarderCreator = () =>
            {
                return _instanceCreator.CreateInstance<CommandDownForwarder<CommandIn, CommandOut, Module>>(commandMapping);
            };
            var handler = new TransientCommandHandler<CommandIn>(forwarderCreator);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        public IInboxDefinition ForwardsCommand<CommandIn, CommandOut, Interceptor, Module>(Func<CommandIn, CommandOut> commandMapping)
            where CommandIn : class, ICommand
            where CommandOut : class, ICommand
            where Interceptor : class, ICommandInterceptor<CommandOut>
            where Module : TychoModule
        {
            Func<CommandDownForwarder<CommandIn, CommandOut, Module>> forwarderCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<CommandDownForwarder<CommandIn, CommandOut, Module>>(commandMapping, interceptor);
            };
            var handler = new TransientCommandHandler<CommandIn>(forwarderCreator);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }
    }
}
