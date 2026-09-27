using SourceHashCode = Tycho.Utils.SourceGenerator.Utils.HashCode;

namespace Tycho.Utils.SourceGenerator.UnitTests.Utils;

public sealed class HashCodeTests
{
    public static TheoryData<int[]> Values => new()
    {
        { [1, 2] },
        { [1, 2, 3] },
        { [1, 2, 3, 4] },
        { [1, 2, 3, 4, 5] },
        { [1, 2, 3, 4, 5, 6] },
        { [1, 2, 3, 4, 5, 6, 7] },
        { [1, 2, 3, 4, 5, 6, 7, 8] },
    };

    [Theory]
    [MemberData(nameof(Values))]
    public void Combine_OverloadsReturnExpectedGrouping(int[] values)
    {
        int result = values.Length switch
        {
            2 => SourceHashCode.Combine(values[0], values[1]),
            3 => SourceHashCode.Combine(values[0], values[1], values[2]),
            4 => SourceHashCode.Combine(values[0], values[1], values[2], values[3]),
            5 => SourceHashCode.Combine(values[0], values[1], values[2], values[3], values[4]),
            6 => SourceHashCode.Combine(values[0], values[1], values[2], values[3], values[4], values[5]),
            7 => SourceHashCode.Combine(values[0], values[1], values[2], values[3], values[4], values[5], values[6]),
            8 => SourceHashCode.Combine(values[0], values[1], values[2], values[3], values[4], values[5], values[6], values[7]),
            _ => throw new ArgumentOutOfRangeException(nameof(values)),
        };

        Assert.Equal(ExpectedCombine(values), result);
    }

    private static int ExpectedCombine(int[] values) => values.Length switch
    {
        2 => CombinePair(values[0], values[1]),
        3 => CombinePair(CombinePair(values[0], values[1]), values[2]),
        4 => CombinePair(CombinePair(values[0], values[1]), CombinePair(values[2], values[3])),
        5 => CombinePair(CombinePair(CombinePair(values[0], values[1]), CombinePair(values[2], values[3])), values[4]),
        6 => CombinePair(
            CombinePair(CombinePair(values[0], values[1]), values[2]),
            CombinePair(CombinePair(values[3], values[4]), values[5])),
        7 => CombinePair(
            CombinePair(CombinePair(values[0], values[1]), CombinePair(values[2], values[3])),
            CombinePair(CombinePair(values[4], values[5]), values[6])),
        8 => CombinePair(
            CombinePair(CombinePair(values[0], values[1]), CombinePair(values[2], values[3])),
            CombinePair(CombinePair(values[4], values[5]), CombinePair(values[6], values[7]))),
        _ => throw new ArgumentOutOfRangeException(nameof(values)),
    };

    private static int CombinePair(int first, int second)
    {
        unchecked
        {
            uint rotated = ((uint)first << 5) | ((uint)first >> 27);
            return ((int)rotated + first) ^ second;
        }
    }
}
