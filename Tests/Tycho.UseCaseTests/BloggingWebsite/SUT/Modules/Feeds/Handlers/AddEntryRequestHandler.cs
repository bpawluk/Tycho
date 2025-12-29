using Tycho.Persistence.EFCore;
using Tycho.Requests;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Domain;
using static Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Contract.AddEntryRequest;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Handlers;

internal class AddEntryRequestHandler(IUnitOfWork unitOfWork, ContentRepository contentRepository) : IRequestHandler<AddEntryRequest, Response>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ContentRepository _contentRepository = contentRepository;

    public async Task<Response> Handle(AddEntryRequest requestData, CancellationToken cancellationToken)
    {
        var entryType = GetEntryType(requestData);
        var entryContent = new Content(requestData.Entry.Author, requestData.Entry.Content);
        var contentId = await _contentRepository.AddEntryContent(entryType, entryContent);

        var feedId = GetFeedId(requestData);
        var feedProvider = new FeedProvider(_unitOfWork);
        var feed = await feedProvider.GetFeed(feedId, cancellationToken);

        var newEntry = feed.AddEntry(entryType, contentId);
        await _unitOfWork.SaveChanges(cancellationToken);

        return new Response(newEntry.Id);
    }

    private static EntryType GetEntryType(AddEntryRequest requestData)
    {
        return requestData.Entry switch
        {
            ArticleEntryData => EntryType.Article,
            PostEntryData => EntryType.Post,
            CommentEntryData => EntryType.Comment,
            _ => throw new ArgumentException("Invalid entry type")
        };
    }

    private static int? GetFeedId(AddEntryRequest requestData)
    {
        return requestData.Entry switch
        {
            ArticleEntryData => null,
            PostEntryData postData => postData.FeedId,
            CommentEntryData commentData => commentData.FeedId,
            _ => throw new ArgumentException("Invalid entry type")
        };
    }
}