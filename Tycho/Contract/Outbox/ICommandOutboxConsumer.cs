using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Outbox
{
    /// <summary>
    /// Lets you define logic for handling command messages sent out by a module that you want to use
    /// </summary>
    public interface ICommandOutboxConsumer
    {
        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <param name="action">A method to be invoked when the command is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleCommand<Command>(Action<Command> action)
            where Command : class, ICommand;

        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <param name="function">A method to be invoked when the command is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleCommand<Command>(Func<Command, Task> function)
            where Command : class, ICommand;

        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <param name="function">A method to be invoked when the command is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleCommand<Command>(Func<Command, CancellationToken, Task> function)
            where Command : class, ICommand;

        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <param name="handler">A handler to be used when the command is received</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleCommand<Command>(ICommandHandler<Command> handler)
            where Command : class, ICommand;

        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// </summary>
        /// <remarks>
        /// Note: A fresh instance of the command handler will be created each time the command is received
        /// </remarks>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <typeparam name="Handler">A handler to be used when the command is received</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer HandleCommand<Command, Handler>()
            where Handler : class, ICommandHandler<Command>
            where Command : class, ICommand;

        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the command</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ForwardCommand<Command, Module>()
            where Command : class, ICommand
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <typeparam name="Interceptor">The type of the message interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the command</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ForwardCommand<Command, Interceptor, Module>()
            where Command : class, ICommand
            where Interceptor : class, ICommandInterceptor<Command>
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="CommandIn">The type of the command being handled</typeparam>
        /// <typeparam name="CommandOut">The type of the command being forwarded</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the command</typeparam>
        /// <param name="mapping">A mapping between the commands</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ForwardCommand<CommandIn, CommandOut, Module>(Func<CommandIn, CommandOut> mapping)
            where CommandIn : class, ICommand
            where CommandOut : class, ICommand
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="CommandIn">The type of the command being handled</typeparam>
        /// <typeparam name="CommandOut">The type of the command being forwarded</typeparam>
        /// <typeparam name="Interceptor">The type of the message interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the command</typeparam>
        /// <param name="mapping">A mapping between the commands</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ForwardCommand<CommandIn, CommandOut, Interceptor, Module>(Func<CommandIn, CommandOut> mapping)
            where CommandIn : class, ICommand
            where CommandOut : class, ICommand
            where Interceptor : class, ICommandInterceptor<CommandOut>
            where Module : TychoModule;

        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeCommand<Command>()
            where Command : class, ICommand;

        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <typeparam name="Interceptor">The type of the message interceptor being used</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeCommand<Command, Interceptor>()
            where Command : class, ICommand
            where Interceptor : class, ICommandInterceptor<Command>;

        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="CommandIn">The type of the command being handled</typeparam>
        /// <typeparam name="CommandOut">The type of the command being exposed</typeparam>
        /// <param name="mapping">A mapping between the commands</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeCommand<CommandIn, CommandOut>(Func<CommandIn, CommandOut> mapping)
            where CommandIn : class, ICommand
            where CommandOut : class, ICommand;

        /// <summary>
        /// Defines logic for handling the specified <b>command</b> message
        /// by exposing it as your module's outgoing message
        /// </summary>
        /// <typeparam name="CommandIn">The type of the command being handled</typeparam>
        /// <typeparam name="CommandOut">The type of the command being exposed</typeparam>
        /// <typeparam name="Interceptor">The type of the message interceptor being used</typeparam>
        /// <param name="mapping">A mapping between the commands</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer ExposeCommand<CommandIn, CommandOut, Interceptor>(Func<CommandIn, CommandOut> mapping)
            where CommandIn : class, ICommand
            where CommandOut : class, ICommand
            where Interceptor : class, ICommandInterceptor<CommandOut>;

        /// <summary>
        /// Ignores the specified <b>command</b> message
        /// </summary>
        /// <typeparam name="Command">The type of the command being ignored</typeparam>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        IOutboxConsumer IgnoreCommand<Command>()
            where Command : class, ICommand;
    }
}
