using Tycho.Identity.Events;
using Tycho.UnitTests._Data.Events;

namespace Tycho.UnitTests.Identity.Events;

public class EventIdentityTests
{
    private static readonly EventIdentity s_eventIdentity = EventIdentity.Create<TestEvent>();

    public static readonly IEnumerable<object[]> EqualsTestData =
    [
        // Same instance => Equal
        [s_eventIdentity, s_eventIdentity, true],

        // Same handler => Equal
        [
            EventIdentity.Create<TestEvent>(),
            EventIdentity.Create<TestEvent>(),
            true
        ],

        // Different handler => Not Equal
        [
            EventIdentity.Create<TestEvent>(),
            EventIdentity.Create<OtherEvent>(),
            false
        ],

        // Comparing to null => Not Equal
        [EventIdentity.Create<TestEvent>(), null!, false]
    ];

    public static readonly IEnumerable<object[]> EqualsObjectTestData = EqualsTestData.Concat(
    [
        // Comparing to an object of a different type => Not Equal
        [EventIdentity.Create<TestEvent>(), new object(), false]
    ]);

    public static readonly IEnumerable<object[]> EqualsOperatorTestData = EqualsTestData.Concat(
    [
        // Comparing two null references => Equal
        [null!, null!, true],

        // Comparing null to an identity => Not Equal
        [null!, EventIdentity.Create<TestEvent>(), false]
    ]);

#pragma warning disable xUnit1042

    [Theory]
    [MemberData(nameof(EqualsTestData))]
    internal void EventIdentity_Equals_EvaluatesCorrectly(EventIdentity left, EventIdentity? right, bool areEqual)
    {
        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsObjectTestData))]
    internal void EventIdentity_EqualsObject_EvaluatesCorrectly(EventIdentity left, object? right, bool areEqual)
    {
        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsOperatorTestData))]
    internal void EventIdentity_EqualsOperator_EvaluatesCorrectly(EventIdentity? left, EventIdentity? right,
        bool areEqual)
    {
        // Act
        bool result = left == right;

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsOperatorTestData))]
    internal void EventIdentity_NotEqualsOperator_EvaluatesCorrectly(EventIdentity? left, EventIdentity? right,
        bool areEqual)
    {
        // Act
        bool result = left != right;

        // Assert
        Assert.Equal(!areEqual, result);
    }

#pragma warning restore xUnit1042
}
