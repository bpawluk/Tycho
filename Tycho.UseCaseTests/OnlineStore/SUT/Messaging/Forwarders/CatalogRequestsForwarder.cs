using Tycho.Apps;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Messaging.Forwarders;

internal static class CatalogRequestsForwarder
{
    public static IAppContract ForwardsCatalogRequests(this IAppContract app)
    {
        app.Forwards<CreateProductRequest, CreateProductRequest.Response, CatalogModule>()
           .Forwards<GetProductsRequest, GetProductsRequest.Response, CatalogModule>();

        return app;
    }
}