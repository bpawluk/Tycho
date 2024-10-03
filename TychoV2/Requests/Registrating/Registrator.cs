using Microsoft.Extensions.DependencyInjection;
using TychoV2.Requests.Registrating.Registrations;
using TychoV2.Structure;

namespace TychoV2.Requests.Registrating
{
    internal partial class Registrator
    {
        private readonly Internals _internals;

        private IServiceCollection Services => _internals.GetServiceCollection();

        public Registrator(Internals internals)
        {
            _internals = internals;
        }

        private bool TryAddRegistration<THandlerRegistrationInterface, THandlerRegistration>()
            where THandlerRegistrationInterface : class, IHandlerRegistration
            where THandlerRegistration : class, THandlerRegistrationInterface
        {
            if (_internals.HasService<THandlerRegistrationInterface>())
            {
                return false;
            }

            Services.AddTransient<THandlerRegistrationInterface, THandlerRegistration>();
            return true;
        }
    }
}
