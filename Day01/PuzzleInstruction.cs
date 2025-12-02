using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AdventOfCode2025.Day01;

public partial record PuzzleInstruction : IParsable<PuzzleInstruction>
{
    [GeneratedRegex(@"^(?<dir>[LR])(?<turns>\d+)$")]
    private static partial Regex LineRegex();

    public TurnDirection Direction { get; init; }
    public long Turns { get; init; }

    public static PuzzleInstruction Parse(string input, IFormatProvider? provider)
    {
        var match = LineRegex().Match(input);

        if (!match.Success)
            throw new ArgumentOutOfRangeException(nameof(input), "Invalid instruction");

        return new PuzzleInstruction()
        {
            Direction = (TurnDirection)match.Groups["dir"].Value.ToUpper()[0],
            Turns = long.Parse(match.Groups["turns"].Value)
        };
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out PuzzleInstruction result) => throw new NotImplementedException();
}
