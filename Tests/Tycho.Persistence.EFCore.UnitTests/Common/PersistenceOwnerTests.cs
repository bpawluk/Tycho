using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Moq;
using Tycho.Persistence.EFCore.Common;
using Tycho.Persistence.EFCore.UnitTests._Utils;
using Tycho.Structure;
using Tycho.Identity.Structure;

namespace Tycho.Persistence.EFCore.UnitTests.Common;

public sealed class PersistenceOwnerTests
{
    [Fact]
    public void Identifier_ForSameModuleInDifferentApplications_IsDifferent()
    {
        // Arrange
        using Internals first = PersistenceTestInternals.Create(typeof(PersistenceOwnerTests), typeof(FirstApp));
        using Internals second = PersistenceTestInternals.Create(typeof(PersistenceOwnerTests), typeof(SecondApp));

        // Act
        string firstIdentifier = new PersistenceOwner(first).Identifier;
        string secondIdentifier = new PersistenceOwner(second).Identifier;

        // Assert
        Assert.Equal(first.OwnerId, second.OwnerId);
        Assert.NotEqual(firstIdentifier, secondIdentifier);
    }

    [Fact]
    public void Identifier_ForReplicasOfSameApplicationAndModule_IsStableAndFixedWidth()
    {
        // Arrange
        using Internals first = PersistenceTestInternals.Create(typeof(PersistenceOwnerTests), typeof(FirstApp));
        using Internals second = PersistenceTestInternals.Create(typeof(PersistenceOwnerTests), typeof(FirstApp));

        // Act
        string identifier = new PersistenceOwner(first).Identifier;

        // Assert
        Assert.Equal(identifier, new PersistenceOwner(second).Identifier);
        Assert.Matches("^[0-9A-F]{32}$", identifier);
    }

    [Fact]
    public void Identifier_ForDifferentModulesInSameApplication_IsDifferent()
    {
        // Arrange
        using Internals first = PersistenceTestInternals.Create(typeof(PersistenceOwnerTests), typeof(FirstApp));
        using Internals second = PersistenceTestInternals.Create(typeof(PersistenceOwner), typeof(FirstApp));

        // Act
        string firstIdentifier = new PersistenceOwner(first).Identifier;
        string secondIdentifier = new PersistenceOwner(second).Identifier;

        // Assert
        Assert.NotEqual(firstIdentifier, secondIdentifier);
    }

    [Fact]
    public void Identifier_WithAmbiguousDelimiterPairs_KeepsOwnersSeparate()
    {
        // Arrange
        using var first = new TestInternals("a:b", "c");
        using var second = new TestInternals("a", "b:c");

        // Act
        string firstIdentifier = new PersistenceOwner(first).Identifier;
        string secondIdentifier = new PersistenceOwner(second).Identifier;

        // Assert
        Assert.NotEqual(firstIdentifier, secondIdentifier);
    }

    [Fact]
    public void Constructor_LogsOwnerIdentityAndConfiguredKey()
    {
        // Arrange
        Internals internals = PersistenceTestInternals.Create(typeof(PersistenceOwnerTests));
        var logger = new Mock<ILogger<PersistenceOwner>>();

        logger
            .Setup(item => item.IsEnabled(LogLevel.Information))
            .Returns(true);

        // Act
        var owner = new PersistenceOwner(internals, logger.Object);

        // Assert
        LogAssert.Logged(logger, LogLevel.Information, 2001, "PersistenceOwnerConfigured", null, ("OwnerId", internals.OwnerId.Value), ("Identifier", owner.Identifier));
    }

    private sealed class TestInternals(string applicationId, string ownerId) : Internals(
        Host.CreateEmptyApplicationBuilder(null),
        new ControlPlane(InstanceIdentity.Parse(applicationId)),
        InstanceIdentity.Parse(ownerId))
    {
        protected override void PrepareForStart(CancellationToken cancellationToken) { }
    }

    private sealed class FirstApp { }
    private sealed class SecondApp { }
}
