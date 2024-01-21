using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Inbox
{
    /// <summary>
    /// Lets you define incoming command messages that your module will handle.
    /// </summary>
    public interface ICommandInboxDefinition
    {
        /// <summary>
        /// Declares that the specified <b>command</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <param name="action">A method to be invoked when the command is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Executes<Command>(Action<Command> action)
            where Command : class, ICommand;

        /// <summary>
        /// Declares that the specified <b>command</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <param name="function">A method to be invoked when the command is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Executes<Command>(Func<Command, Task> function)
            where Command : class, ICommand;

        /// <summary>
        /// Declares that the specified <b>command</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <param name="function">A method to be invoked when the command is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Executes<Command>(Func<Command, CancellationToken, Task> function)
            where Command : class, ICommand;

        /// <summary>
        /// Declares that the specified <b>command</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <param name="handler">A handler to be used when the command is received</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Executes<Command>(ICommandHandler<Command> handler)
            where Command : class, ICommand;

        /// <summary>
        /// Declares that the specified <b>command</b> message is handled by your module 
        /// and defines logic for doing so
        /// </summary>
        /// <remarks>
        /// Note: A fresh instance of the command handler will be created each time the command is received
        /// </remarks>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <typeparam name="Handler">A handler to be used when the command is received</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition Executes<Command, Handler>()
            where Handler : class, ICommandHandler<Command>
            where Command : class, ICommand;

        /// <summary>
        /// Declares that the specified <b>command</b> message is handled by your module 
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the command</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition ForwardsCommand<Command, Module>()
            where Command : class, ICommand
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>command</b> message is handled by your module 
        /// by forwarding it to the specified submodule
        /// </summary>
        /// <typeparam name="Command">The type of the command being handled</typeparam>
        /// <typeparam name="Interceptor">The type of the message interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the command</typeparam>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition ForwardsCommand<Command, Interceptor, Module>()
            where Command : class, ICommand
            where Interceptor : class, ICommandInterceptor<Command>
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>command</b> message is handled by your module 
        /// by forwarding it to the specified submodule as another command
        /// </summary>
        /// <typeparam name="CommandIn">The type of the command being handled</typeparam>
        /// <typeparam name="CommandOut">The type of the command being forwarded</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the command</typeparam>
        /// <param name="mapping">A mapping between the commands</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition ForwardsCommand<CommandIn, CommandOut, Module>(Func<CommandIn, CommandOut> mapping)
            where CommandIn : class, ICommand
            where CommandOut : class, ICommand
            where Module : TychoModule;

        /// <summary>
        /// Declares that the specified <b>command</b> message is handled by your module 
        /// by forwarding it to the specified submodule as another command
        /// </summary>
        /// <typeparam name="CommandIn">The type of the command being handled</typeparam>
        /// <typeparam name="CommandOut">The type of the command being forwarded</typeparam>
        /// <typeparam name="Interceptor">The type of the message interceptor being used</typeparam>
        /// <typeparam name="Module">The type of the submodule to receive the command</typeparam>
        /// <param name="mapping">A mapping between the commands</param>
        /// <exception cref="ArgumentException"/>
        IInboxDefinition ForwardsCommand<CommandIn, CommandOut, Interceptor, Module>(Func<CommandIn, CommandOut> mapping)
            where CommandIn : class, ICommand
            where CommandOut : class, ICommand
            where Interceptor : class, ICommandInterceptor<CommandOut>
            where Module : TychoModule;
    }
}
