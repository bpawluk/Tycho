using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Domain;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Handlers;

internal class AddUserRequestHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddUserRequest, AddUserRequest.Response>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<AddUserRequest.Response> Handle(AddUserRequest requestData, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken); // Simulate async work
        var users = _unitOfWork.Set<User>();
        var newUser = new User(requestData.Name);
        users.Add(newUser);
        await _unitOfWork.SaveChanges(cancellationToken);
        return new(newUser.Id);
    }
}