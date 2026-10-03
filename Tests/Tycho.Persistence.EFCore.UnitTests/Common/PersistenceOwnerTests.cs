using Microsoft.Extensions.Logging;
using Moq;
using Tycho.Persistence.EFCore.Common;
using Tycho.Persistence.EFCore.UnitTests._Utils;
using Tycho.Structure;

namespace Tycho.Persistence.EFCore.UnitTests.Common;

public sealed class PersistenceOwnerTests
{
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
        LogAssert.Logged(logger, LogLevel.Information, 2001, "PersistenceOwnerConfigured", null, ("OwnerInstanceId", internals.OwnerInstanceId.Value), ("Key", owner.Key));
    }
}
