using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.UseCaseTests.ContentModeration.SUT.Mappers;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Incoming;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Contract.Outgoing;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Contract;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Contract;

namespace Tycho.UseCaseTests.ContentModeration.SUT;

public class ContentModerationApp : TychoApp
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

    protected override void MapEvents(IAppEvents app)
    {
        app.Routes<UserBannedEvent>()
           .ForwardsAs<UserStatusChangedEvent, UsersModule>(EventMapper.Map);

        app.Routes<PostRemovedEvent>()
           .ForwardsAs<PostStatusChangedEvent, PostsModule>(EventMapper.Map);
    }

    protected override void RegisterServices(IServiceCollection app) { }
}