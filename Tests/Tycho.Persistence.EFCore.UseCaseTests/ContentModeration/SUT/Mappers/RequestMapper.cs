using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Outgoing;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Mappers;

internal static class RequestMapper
{
    public static GetPostRequest MapRequest(GetAuthorRequest requestData)
    {
        return new(requestData.PostId);
    }

    public static GetAuthorRequest.Response MapResponse(GetPostRequest.Response responseData)
    {
        return new(responseData.Post.AuthorId);
    }
}
