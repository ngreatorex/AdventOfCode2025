using AdventOfCode2025.Shared.Maths;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode2025.Day09;

public record RedTile : IParsable<RedTile>
{
    public int X { get; init; }
    public int Y { get; init; }

    public override string ToString() => $"({X},{Y})";

    public Point Point => new(X, Y);

    public static RedTile Parse(string s, IFormatProvider? provider)
    {
        var parts = s.Split(',');
        if (parts.Length != 2)
            throw new ArgumentOutOfRangeException(nameof(s));

        return new RedTile
        {
            X = int.Parse(parts[0]),
            Y = int.Parse(parts[1])
        };
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out RedTile result) => throw new NotImplementedException();
}
