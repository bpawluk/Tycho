using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Mappers;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Incoming;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Outgoing;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT;

[TychoDefinition]
public partial class ContentModerationApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.Forwards<AddUserRequest, AddUserRequest.Response, UsersModule>()
           .Forwards<GetUsersRequest, GetUsersRequest.Response, UsersModule>();

        app.Forwards<AddPostRequest, AddPostRequest.Response, PostsModule>()
           .Forwards<GetPostRequest, GetPostRequest.Response, PostsModule>()
           .Forwards<GetPostsRequest, GetPostsRequest.Response, PostsModule>();

        app.Forwards<RemovePostRequest, AdminModule>();
    }

    protected override void DefineEvents(IAppEvents app)
    {
        app.Routes<UserBannedEvent>()
           .ForwardsAs<UserStatusChangedEvent, UsersModule>(EventMapper.Map);

        app.Routes<PostRemovedEvent>()
           .ForwardsAs<PostStatusChangedEvent, PostsModule>(EventMapper.Map);
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<UsersModule>()
           .Uses<PostsModule>();

        app.Uses<AdminModule>(outgoingRequests =>
        {
            outgoingRequests.ForwardAs<
                GetAuthorRequest, GetAuthorRequest.Response,
                GetPostRequest, GetPostRequest.Response,
                PostsModule>(RequestMapper.Map, RequestMapper.Map);
        });
    }

    protected override void RegisterServices(IServiceCollection app) { }
}
