using System;

namespace Tycho.DependencyInjection
{
    internal class StubServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType) => default!;
    }
}
