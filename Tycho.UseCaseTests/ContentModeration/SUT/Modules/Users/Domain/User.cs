namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Users.Domain;

internal class User(string name)
{
    public int Id { get; private set; }

    public string Name { get; private set; } = name;

    public UserStatus Status { get; set; } = UserStatus.Active;

    public enum UserStatus
    {
        Active,
        Deactivated
    }
}