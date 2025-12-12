using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AdventOfCode2025.Day10;

public partial class Button
{
    public IEnumerable<int> ToggledLightIndexes => Enumerable.Range(0, ToggleMask.Length).Where(ToggleMask.Get);

    public required BitArray ToggleMask { get; init; }

    public BitArray Apply(BitArray input) => new BitArray(input).Xor(ToggleMask);

    public List<int> ApplyJoltages(List<int> input)
    {
        var output = new List<int>(input);
        Debug.Assert(input.Count == ToggleMask.Length);

        for (var i = 0; i < ToggleMask.Length; i++)
        {
            if (ToggleMask.Get(i))
                output[i] = output[i] + 1;
        }

        return output;
    }

    public override string ToString() => $"({string.Join(",", ToggledLightIndexes)})";

    public static Button Parse(string s, int lightCount)
    {
        var match = ButtonRegex().Match(s);

        if (!match.Success)
            throw new ArgumentOutOfRangeException(nameof(s));

        var indexes = match.Groups["lights"].Value.Split(',').Select(int.Parse).ToList();
        var bitArray = new BitArray(lightCount);

        foreach (var index in indexes)
            bitArray.Set(index, true);

        return new Button
        {
            ToggleMask = bitArray
        };
    }

    [GeneratedRegex(@"\((?<lights>\d+(,\d+)*)\)")]
    private static partial Regex ButtonRegex();
}

