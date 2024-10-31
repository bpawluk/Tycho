using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Domain;
using static Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Contract.CreateProductRequest;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Handlers;

internal class CreateProductRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateProductRequest, Response>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Response> Handle(CreateProductRequest requestData, CancellationToken cancellationToken)
    {
        var products = _unitOfWork.Set<Product>();
        var newProduct = new Product(requestData.Name, requestData.Price);
        products.Add(newProduct);
        await _unitOfWork.SaveChanges(cancellationToken);
        return new Response(newProduct.Id);
    }
}