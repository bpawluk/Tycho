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
        app.Expects<AddUserRequest, AddUserRequest.Response>()
           .ForwardsTo<UsersModule>();

        app.Expects<GetUsersRequest, GetUsersRequest.Response>()
           .ForwardsTo<UsersModule>();

        app.Expects<AddPostRequest, AddPostRequest.Response>()
           .ForwardsTo<PostsModule>();

        app.Expects<GetPostRequest, GetPostRequest.Response>()
           .ForwardsTo<PostsModule>();

        app.Expects<GetPostsRequest, GetPostsRequest.Response>()
           .ForwardsTo<PostsModule>();

        app.Expects<RemovePostRequest>()
           .ForwardsTo<AdminModule>();
    }

    protected override void DefineEvents(IAppEvents app)
    {
        app.Expects<UserBannedEvent>()
           .MapsTo<UserStatusChangedEvent>(EventMapper.Map)
           .ForwardsTo<UsersModule>();

        app.Expects<PostRemovedEvent>()
           .MapsTo<PostStatusChangedEvent>(EventMapper.Map)
           .ForwardsTo<PostsModule>();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<UsersModule>();

        app.Uses<PostsModule>();

        app.Uses<AdminModule>(app =>
        {
            app.Fulfills<GetAuthorRequest, GetAuthorRequest.Response>()
               .MapsTo<GetPostRequest, GetPostRequest.Response>(
                    RequestMapper.MapRequest,
                    RequestMapper.MapResponse)
               .ForwardsTo<PostsModule>();
        });
    }

    protected override void RegisterServices(IServiceCollection app) { }
}
