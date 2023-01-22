using SampleApp.Catalog;
using SampleApp.Catalog.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace SampleApp.App.Handlers;

internal class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, IEnumerable<Product>>
{
    private readonly ISubmodule<CatalogModule> _catalogModule;

    public GetProductsQueryHandler(ISubmodule<CatalogModule> catalogModule)
    {
        _catalogModule = catalogModule;
    }

    public Task<IEnumerable<Product>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        return _catalogModule.Execute<GetProductsQuery, IEnumerable<Product>>(query, cancellationToken);
    }
}
