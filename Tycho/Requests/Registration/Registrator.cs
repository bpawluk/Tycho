using System;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Requests.Registration.Registrations;
using Tycho.Structure;

namespace Tycho.Requests.Registration
{
    internal partial class Registrator
    {
        private readonly Internals _internals;

        private IServiceCollection Services => _internals.GetHostBuilder().Services;

        public Registrator(Internals internals)
        {
            _internals = internals;
        }

        private bool TryAddRegistration<THandlerRegistrationInterface, THandlerRegistration>()
            where THandlerRegistrationInterface : class, IRequestRegistration
            where THandlerRegistration : class, THandlerRegistrationInterface
        {
            if (_internals.HasService<THandlerRegistrationInterface>())
            {
                return false;
            }

            Services.AddTransient<THandlerRegistrationInterface, THandlerRegistration>();
            return true;
        }

        private bool TryAddRegistration<THandlerRegistrationInterface, THandlerRegistration>(
            Func<IServiceProvider, THandlerRegistration> registrationFactory)
            where THandlerRegistrationInterface : class, IRequestRegistration
            where THandlerRegistration : class, THandlerRegistrationInterface
        {
            if (_internals.HasService<THandlerRegistrationInterface>())
            {
                return false;
            }

            Services.AddTransient<THandlerRegistrationInterface>(registrationFactory);
            return true;
        }
    }
}
