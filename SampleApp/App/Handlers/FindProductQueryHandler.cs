using SampleApp.Catalog;
using SampleApp.Catalog.Model;
using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace SampleApp.App.Handlers;

internal class FindProductQueryHandler : IQueryHandler<FindProductQuery, Product>
{
    private readonly ISubmodule<CatalogModule> _catalogModule;

    public FindProductQueryHandler(ISubmodule<CatalogModule> catalogModule)
    {
        _catalogModule = catalogModule;
    }

    public Task<Product> Handle(FindProductQuery query, CancellationToken cancellationToken)
    {
        return _catalogModule.Execute<FindProductQuery, Product>(query, cancellationToken);
    }
}
