using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Outgoing;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Mappers;

internal static class RequestMapper
{
    public static GetPostRequest Map(GetAuthorRequest requestData)
    {
        return new(requestData.PostId);
    }

    public static GetAuthorRequest.Response Map(GetPostRequest.Response responseData)
    {
        return new(responseData.Post.AuthorId);
    }
}