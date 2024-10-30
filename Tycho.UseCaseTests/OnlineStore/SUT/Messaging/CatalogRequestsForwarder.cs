using Tycho.Apps;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Messaging;

internal static class CatalogRequestsForwarder
{
    public static IAppContract ForwardsCatalogRequests(this IAppContract app)
    {
        return app;
    }
}