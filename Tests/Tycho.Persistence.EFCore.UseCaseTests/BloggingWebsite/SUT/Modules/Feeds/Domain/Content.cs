namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Domain;

internal record Content(int Id, string Author, string Value)
{
    public Content(string author, string value) : this(-1, author, value) { }
}