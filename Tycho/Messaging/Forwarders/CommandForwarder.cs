using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Forwarders
{
    internal abstract class CommandForwarder<CommandIn, CommandOut>
        : ForwarderBase<CommandIn, CommandOut>
        , ICommandHandler<CommandIn>
        where CommandIn : class, IRequest
        where CommandOut : class, IRequest
    {
        private readonly ICommandInterceptor<CommandOut>? _interceptor;

        public CommandForwarder(
            IModule target,
            Func<CommandIn, CommandOut> mapping,
            ICommandInterceptor<CommandOut>? interceptor)
            : base(target, mapping)
        {
            _interceptor = interceptor;
        }

        public async Task Handle(CommandIn commandData, CancellationToken cancellationToken)
        {
            var mappedCommand = _messageMapping(commandData);

            if (_interceptor != null)
            {
                await _interceptor.ExecuteBefore(mappedCommand, cancellationToken).ConfigureAwait(false);
            }

            await _target.Execute(mappedCommand, cancellationToken).ConfigureAwait(false);

            if (_interceptor != null)
            {
                await _interceptor.ExecuteAfter(mappedCommand, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    internal class CommandUpForwarder<CommandIn, CommandOut>
        : CommandForwarder<CommandIn, CommandOut>
        , ICommandHandler<CommandIn>
        where CommandIn : class, IRequest
        where CommandOut : class, IRequest
    {
        public CommandUpForwarder(
            IModule module,
            Func<CommandIn, CommandOut> mapping,
            ICommandInterceptor<CommandOut>? interceptor = null)
            : base(module, mapping, interceptor) { }
    }

    internal class CommandDownForwarder<CommandIn, CommandOut, Module>
        : CommandForwarder<CommandIn, CommandOut>
        , ICommandHandler<CommandIn>
        where CommandIn : class, IRequest
        where CommandOut : class, IRequest
        where Module : TychoModule
    {
        public CommandDownForwarder(
            IModule<Module> submodule,
            Func<CommandIn, CommandOut> mapping,
            ICommandInterceptor<CommandOut>? interceptor = null)
            : base(submodule, mapping, interceptor) { }
    } 
}
