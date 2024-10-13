using Tycho.Events.Routing;

namespace Tycho.UnitTests.Events.Routing;

public class HandlerIdentityTests
{
    private readonly static HandlerIdentity _handlerIdentity = new("EventOne", "HandlerOne", "ModuleOne");

    public readonly static IEnumerable<object[]> EqualsTestData =
    [
        [_handlerIdentity, _handlerIdentity, true],
        [new HandlerIdentity("EventOne", "HandlerOne", "ModuleOne"), new HandlerIdentity("EventOne", "HandlerOne", "ModuleOne"), true],
        [new HandlerIdentity("EventOne", "HandlerOne", "ModuleOne"), new HandlerIdentity("EventTwo", "HandlerOne", "ModuleOne"), false],
        [new HandlerIdentity("EventOne", "HandlerOne", "ModuleOne"), new HandlerIdentity("EventOne", "HandlerTwo", "ModuleOne"), false],
        [new HandlerIdentity("EventOne", "HandlerOne", "ModuleOne"), new HandlerIdentity("EventOne", "HandlerOne", "ModuleTwo"), false],
        [new HandlerIdentity("EventOne", "HandlerOne", "ModuleOne"), new HandlerIdentity("EventTwo", "HandlerTwo", "ModuleOne"), false],
        [new HandlerIdentity("EventOne", "HandlerOne", "ModuleOne"), new HandlerIdentity("EventOne", "HandlerTwo", "ModuleTwo"), false],
        [new HandlerIdentity("EventOne", "HandlerOne", "ModuleOne"), new HandlerIdentity("EventTwo", "HandlerOne", "ModuleTwo"), false],
        [new HandlerIdentity("EventOne", "HandlerOne", "ModuleOne"), new HandlerIdentity("EventTwo", "HandlerTwo", "ModuleTwo"), false],
        [new HandlerIdentity("EventOne", "HandlerOne", "ModuleOne"), null!, false],
    ];

    public readonly static IEnumerable<object[]> EqualsObjectTestData = EqualsTestData.Concat(
    [
        [new HandlerIdentity("EventOne", "HandlerOne", "ModuleOne"), new object(), false]
    ]);

    public readonly static IEnumerable<object[]> EqualsOperatorTestData = EqualsTestData.Concat(
    [
        [null!, null!, true],
        [null!, new HandlerIdentity("EventOne", "HandlerOne", "ModuleOne"), false]
    ]);

    [Theory]
    [MemberData(nameof(EqualsTestData))]
    internal void HandlerIdentity_Equals_EvaluatesCorrectly(HandlerIdentity left, HandlerIdentity? right, bool areEqual)
    {
        // Act
        var result = left.Equals(right);

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsObjectTestData))]
    internal void HandlerIdentity_EqualsObject_EvaluatesCorrectly(HandlerIdentity left, object? right, bool areEqual)
    {
        // Act
        var result = left.Equals(right);

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsOperatorTestData))]
    internal void HandlerIdentity_EqualsOperator_EvaluatesCorrectly(HandlerIdentity? left, HandlerIdentity? right, bool areEqual)
    {
        // Act
        var result = left == right;

        // Assert
        Assert.Equal(areEqual, result);
    }

    [Theory]
    [MemberData(nameof(EqualsOperatorTestData))]
    internal void HandlerIdentity_NotEqualsOperator_EvaluatesCorrectly(HandlerIdentity? left, HandlerIdentity? right, bool areEqual)
    {
        // Act
        var result = left != right;

        // Assert
        Assert.Equal(!areEqual, result);
    }
}
