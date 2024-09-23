using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using TychoV2.Requests.Registrations;
using TychoV2.Structure;

namespace TychoV2.Requests.Registrator
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
            if (Services.Any(service => service.ServiceType == typeof(THandlerRegistrationInterface)))
            {
                Services.AddTransient<THandlerRegistrationInterface, THandlerRegistration>();
                return true;
            }

            return false;
        }
    }
}
