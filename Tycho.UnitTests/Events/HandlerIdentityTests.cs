using TychoV2.Events.Routing;

namespace Tycho.UnitTests.Events;

public class HandlerIdentityTests
{
    private readonly static HandlerIdentity _handlerIdentity = new("ModuleOne", "HandlerOne");  

    public readonly static IEnumerable<object[]> EqualsTestData =
    [
        [_handlerIdentity, _handlerIdentity, true],
        [new HandlerIdentity("ModuleOne", "HandlerOne"), new HandlerIdentity("ModuleOne", "HandlerOne"), true],
        [new HandlerIdentity("ModuleOne", "HandlerOne"), new HandlerIdentity("ModuleTwo", "HandlerOne"), false],
        [new HandlerIdentity("ModuleOne", "HandlerOne"), new HandlerIdentity("ModuleOne", "HandlerTwo"), false],
        [new HandlerIdentity("ModuleOne", "HandlerOne"), new HandlerIdentity("ModuleTwo", "HandlerTwo"), false],
        [new HandlerIdentity("ModuleOne", "HandlerOne"), null!, false],
    ];

    public readonly static IEnumerable<object[]> EqualsObjectTestData = EqualsTestData.Concat(
    [
        [new HandlerIdentity("ModuleOne", "HandlerOne"), new object(), false]
    ]);

    public readonly static IEnumerable<object[]> EqualsOperatorTestData = EqualsTestData.Concat(
    [
        [null!, null!, true],
        [null!, new HandlerIdentity("ModuleOne", "HandlerOne"), false]
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
