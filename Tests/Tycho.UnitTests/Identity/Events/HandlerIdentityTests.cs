using Tycho.Identity.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Identity.Events;

public class EventHandlerIdentityTests
{
    private static readonly EventHandlerIdentity _handlerIdentity = EventHandlerIdentity.Create<TestEventHandler>();

    public static readonly IEnumerable<object[]> EqualsTestData =
    [
        // Same instance => Equal
        [_handlerIdentity, _handlerIdentity, true],

        // Same handler type => Equal
        [
            EventHandlerIdentity.Create<TestEventHandler>(),
            EventHandlerIdentity.Create<TestEventHandler>(),
            true
        ],

        // Different handler type => Not Equal
        [
            EventHandlerIdentity.Create<TestEventHandler>(),
            EventHandlerIdentity.Create<OtherEventHandler>(),
            false
        ],

        // Comparing to null => Not Equal
        [EventHandlerIdentity.Create<TestEventHandler>(), null!, false]
    ];

    public static readonly IEnumerable<object[]> EqualsObjectTestData = EqualsTestData.Concat(
    [
        // Comparing to an object of a different type => Not Equal
        [EventHandlerIdentity.Create<TestEventHandler>(), new object(), false]
    ]);

    public static readonly IEnumerable<object[]> EqualsOperatorTestData = EqualsTestData.Concat(
    [
        // Comparing two null references => Equal
        [null!, null!, true],

        // Comparing null to an identity => Not Equal
        [null!, EventHandlerIdentity.Create<TestEventHandler>(), false]
    ]);

    [Theory]
    [MemberData(nameof(EqualsTestData))]
    internal void EventHandlerIdentity_Equals_EvaluatesCorrectly(EventHandlerIdentity left, EventHandlerIdentity? right, bool areEqual)
    {
        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsObjectTestData))]
    internal void EventHandlerIdentity_EqualsObject_EvaluatesCorrectly(EventHandlerIdentity left, object? right, bool areEqual)
    {
        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsOperatorTestData))]
    internal void EventHandlerIdentity_EqualsOperator_EvaluatesCorrectly(EventHandlerIdentity? left, EventHandlerIdentity? right,
        bool areEqual)
    {
        // Act
        bool result = left == right;

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsOperatorTestData))]
    internal void EventHandlerIdentity_NotEqualsOperator_EvaluatesCorrectly(EventHandlerIdentity? left, EventHandlerIdentity? right,
        bool areEqual)
    {
        // Act
        bool result = left != right;

        // Assert
        Assert.Equal(!areEqual, result);
    }
}
