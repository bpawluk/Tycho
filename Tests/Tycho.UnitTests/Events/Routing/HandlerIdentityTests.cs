using Tycho.Identity.Events;
using Tycho.UnitTests._Data.Events;
using Tycho.UnitTests._Data.Handlers;

namespace Tycho.UnitTests.Events.Routing;

public class EventHandlerIdentityTests
{
    private static readonly EventHandlerIdentity _handlerIdentity = EventHandlerIdentity.Create<TestEventHandler, TestEvent>();

    public static readonly IEnumerable<object[]> EqualsTestData =
    [
        // Same instance => Equal
        [_handlerIdentity, _handlerIdentity, true],

        // Same handler and event types => Equal
        [
            EventHandlerIdentity.Create<TestEventHandler, TestEvent>(),
            EventHandlerIdentity.Create<TestEventHandler, TestEvent>(), 
            true
        ],

        // Same handler type but different event types => Not Equal
        [
            EventHandlerIdentity.Create<TestEventHandler, TestEvent>(),
            EventHandlerIdentity.Create<TestEventHandler, OtherEvent>(), 
            false
        ],

        // Same event type but different handler types => Not Equal
        [
            EventHandlerIdentity.Create<MultiEventHandler, TestEvent>(),
            EventHandlerIdentity.Create<MultiEventHandler, OtherEvent>(), 
            false
        ],

        // Different handler and event types => Not Equal
        [
            EventHandlerIdentity.Create<TestEventHandler, TestEvent>(),
            EventHandlerIdentity.Create<OtherEventHandler, OtherEvent>(), 
            false
        ],

        // Comparing to null => Not Equal
        [EventHandlerIdentity.Create<TestEventHandler, TestEvent>(), null!, false]
    ];

    public static readonly IEnumerable<object[]> EqualsObjectTestData = EqualsTestData.Concat(
    [
        // Comparing to an object of a different type => Not Equal
        [EventHandlerIdentity.Create<TestEventHandler, TestEvent>(), new object(), false]
    ]);

    public static readonly IEnumerable<object[]> EqualsOperatorTestData = EqualsTestData.Concat(
    [
        // Comparing two null references => Equal
        [null!, null!, true],

        // Comparing null to an identity => Not Equal
        [null!, EventHandlerIdentity.Create<TestEventHandler, TestEvent>(), false]
    ]);

    [Theory]
    [MemberData(nameof(EqualsTestData))]
    internal void EventHandlerIdentity_Equals_EvaluatesCorrectly(EventHandlerIdentity left, EventHandlerIdentity? right, bool areEqual)
    {
        // Act
        var result = left.Equals(right);

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsObjectTestData))]
    internal void EventHandlerIdentity_EqualsObject_EvaluatesCorrectly(EventHandlerIdentity left, object? right, bool areEqual)
    {
        // Act
        var result = left.Equals(right);

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsOperatorTestData))]
    internal void EventHandlerIdentity_EqualsOperator_EvaluatesCorrectly(EventHandlerIdentity? left, EventHandlerIdentity? right,
        bool areEqual)
    {
        // Act
        var result = left == right;

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsOperatorTestData))]
    internal void EventHandlerIdentity_NotEqualsOperator_EvaluatesCorrectly(EventHandlerIdentity? left, EventHandlerIdentity? right,
        bool areEqual)
    {
        // Act
        var result = left != right;

        // Assert
        Assert.Equal(!areEqual, result);
    }
}