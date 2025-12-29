namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Domain;

internal class AdminAction
{
    public int Id { get; private set; }

    public int RemovedPostId { get; private set; }

    public int? BannedUserId { get; private set; }

    private AdminAction(int removedPostId, int? bannedUserId)
    {
        RemovedPostId = removedPostId;
        BannedUserId = bannedUserId;
    }

    public static AdminAction RemovePost(int postId)
    {
        return new AdminAction(postId, null);
    }

    public static AdminAction RemovePostAndBanAuthor(int postId, int authorId)
    {
        return new AdminAction(postId, authorId);
    }
}