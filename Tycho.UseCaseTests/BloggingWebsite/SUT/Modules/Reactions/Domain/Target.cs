namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Domain;

internal class Target(int id)
{
    public int Id { get; private set; } = id;

    public uint Score { get; private set; } = 0;

    public void AddReaction()
    {
        Score++;
    }
}