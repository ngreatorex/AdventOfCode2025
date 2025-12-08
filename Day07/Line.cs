using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode2025.Day07;

public record Line : IParsable<Line>
{
    public char[] Cells { get; init; } = [];

    public static Line Parse(string s, IFormatProvider? provider)
    {
        return new Line
        {
            Cells = [.. s]
        };
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Line result) => throw new NotImplementedException();
}
