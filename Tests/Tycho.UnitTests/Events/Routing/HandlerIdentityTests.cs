using Tycho.Identity.Events;

namespace Tycho.UnitTests.Events.Routing;

public class EventHandlerIdentityTests
{
    private static readonly EventHandlerIdentity _handlerIdentity = new("EventOne", "HandlerOne");

    public static readonly IEnumerable<object[]> EqualsTestData =
    [
        [_handlerIdentity, _handlerIdentity, true],
        [
            new EventHandlerIdentity("EventOne", "HandlerOne"),
            new EventHandlerIdentity("EventOne", "HandlerOne"), 
            true
        ],
        [
            new EventHandlerIdentity("EventOne", "HandlerOne"),
            new EventHandlerIdentity("EventTwo", "HandlerOne"), 
            false
        ],
        [
            new EventHandlerIdentity("EventOne", "HandlerOne"),
            new EventHandlerIdentity("EventOne", "HandlerTwo"), 
            false
        ],
        [
            new EventHandlerIdentity("EventOne", "HandlerOne"),
            new EventHandlerIdentity("EventTwo", "HandlerTwo"), 
            false
        ],
        [new EventHandlerIdentity("EventOne", "HandlerOne"), null!, false]
    ];

    public static readonly IEnumerable<object[]> EqualsObjectTestData = EqualsTestData.Concat(
    [
        [new EventHandlerIdentity("EventOne", "HandlerOne"), new object(), false]
    ]);

    public static readonly IEnumerable<object[]> EqualsOperatorTestData = EqualsTestData.Concat(
    [
        [null!, null!, true],
        [null!, new EventHandlerIdentity("EventOne", "HandlerOne"), false]
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