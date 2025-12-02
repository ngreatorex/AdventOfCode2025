using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AdventOfCode2025.Day02;

public partial record PuzzleInstruction : IParsable<PuzzleInstruction>
{
    [GeneratedRegex(@"^(?<start>\d+)-(?<end>\d+)$")]
    private static partial Regex LineRegex();

    public long StartID { get; set; }
    public long EndID { get; set; }

    public static PuzzleInstruction Parse(string s, IFormatProvider? provider)
    {
        var match = LineRegex().Match(s);

        if (!match.Success)
            throw new ArgumentOutOfRangeException(nameof(s), "Invalid instruction");

        return new PuzzleInstruction()
        {
            StartID = long.Parse(match.Groups["start"].Value),
            EndID = long.Parse(match.Groups["end"].Value)
        };
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out PuzzleInstruction result) => throw new NotImplementedException();
}
