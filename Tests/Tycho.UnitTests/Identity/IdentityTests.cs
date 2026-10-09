using Tycho.Identity.Events;
using Tycho.Identity.Structure;

namespace Tycho.UnitTests.Identity;

public class IdentityTests
{
    [Fact]
    public void Equals_WithDifferentIdentityTypes_ComparesBaseValues()
    {
        // Arrange
        InstanceIdentity instance = InstanceIdentity.Parse("App");
        EventIdentity eventIdentity = EventIdentity.Parse("App");
        Tycho.Identity.Identity sut = instance;

        // Act
        bool result = sut.Equals(eventIdentity);

        // Assert
        Assert.True(result);
        Assert.True(sut.Equals((object)eventIdentity));
        Assert.True(sut == eventIdentity);
        Assert.Equal(sut.GetHashCode(), eventIdentity.GetHashCode());
    }
}
