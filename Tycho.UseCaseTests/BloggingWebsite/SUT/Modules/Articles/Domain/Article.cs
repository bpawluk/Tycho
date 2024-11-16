namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Domain;

internal class Article(string author, string content)
{
    public int Id { get; private set; }

    public string Author { get; private set; } = author;

    public string Content { get; private set; } = content;
}