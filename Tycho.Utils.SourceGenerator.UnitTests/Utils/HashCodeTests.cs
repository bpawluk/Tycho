using SourceHashCode = Tycho.Utils.SourceGenerator.Utils.HashCode;

namespace Tycho.Utils.SourceGenerator.UnitTests.Utils;

public sealed class HashCodeTests
{
    [Fact]
    public void Combine_TwoValues_UsesPairHash()
    {
        // Act
        int result = SourceHashCode.Combine(1, 2);

        // Assert
        Assert.Equal(CombinePair(1, 2), result);
    }

    [Fact]
    public void Combine_ThreeValues_GroupsFirstTwoValues()
    {
        // Act
        int result = SourceHashCode.Combine(1, 2, 3);

        // Assert
        Assert.Equal(CombinePair(CombinePair(1, 2), 3), result);
    }

    [Fact]
    public void Combine_FourValues_GroupsIntoTwoPairs()
    {
        // Act
        int result = SourceHashCode.Combine(1, 2, 3, 4);

        // Assert
        Assert.Equal(CombinePair(CombinePair(1, 2), CombinePair(3, 4)), result);
    }

    [Fact]
    public void Combine_FiveValues_GroupsFirstFourValues()
    {
        // Act
        int result = SourceHashCode.Combine(1, 2, 3, 4, 5);

        // Assert
        Assert.Equal(CombinePair(CombinePair(CombinePair(1, 2), CombinePair(3, 4)), 5), result);
    }

    [Fact]
    public void Combine_SixValues_GroupsIntoTwoTriples()
    {
        // Act
        int result = SourceHashCode.Combine(1, 2, 3, 4, 5, 6);

        // Assert
        Assert.Equal(
            CombinePair(
                CombinePair(CombinePair(1, 2), 3),
                CombinePair(CombinePair(4, 5), 6)),
            result);
    }

    [Fact]
    public void Combine_SevenValues_GroupsFourThenThree()
    {
        // Act
        int result = SourceHashCode.Combine(1, 2, 3, 4, 5, 6, 7);

        // Assert
        Assert.Equal(
            CombinePair(
                CombinePair(CombinePair(1, 2), CombinePair(3, 4)),
                CombinePair(CombinePair(5, 6), 7)),
            result);
    }

    [Fact]
    public void Combine_EightValues_GroupsIntoTwoFours()
    {
        // Act
        int result = SourceHashCode.Combine(1, 2, 3, 4, 5, 6, 7, 8);

        // Assert
        Assert.Equal(
            CombinePair(
                CombinePair(CombinePair(1, 2), CombinePair(3, 4)),
                CombinePair(CombinePair(5, 6), CombinePair(7, 8))),
            result);
    }

    private static int CombinePair(int first, int second)
    {
        unchecked
        {
            uint rotated = ((uint)first << 5) | ((uint)first >> 27);
            return ((int)rotated + first) ^ second;
        }
    }
}
