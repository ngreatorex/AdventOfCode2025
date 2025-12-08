using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode2025.Day08;

public class JunctionBox : IParsable<JunctionBox>
{
    public int X { get; init; }
    public int Y { get; init; }
    public int Z { get; init; }

    public override string ToString() => $"({X}, {Y}, {Z})";

    public static JunctionBox Parse(string s, IFormatProvider? provider)
    {
        var parts = s.Split(',');
        if (parts.Length != 3)
            throw new ArgumentOutOfRangeException(nameof(s), "Should have 3 coordinates separated by commas");

        return new JunctionBox
        {
            X = int.Parse(parts[0]),
            Y = int.Parse(parts[1]),
            Z = int.Parse(parts[2])
        };
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out JunctionBox result) => throw new NotImplementedException();
}
