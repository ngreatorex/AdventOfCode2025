using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AdventOfCode2025.Day12;

public record ProblemList(List<Problem> Grids) : Instruction;

public partial record Instruction : IParsable<Instruction>
{
    public static Instruction Parse(string s, IFormatProvider? provider)
    {
        var shapeMatch = ShapeRegex().Match(s);

        if (shapeMatch.Success)
        {
            return new Polyomino
            {
                Number = int.Parse(shapeMatch.Groups["number"].Value),
                Coords = TranslateCoords(shapeMatch.Groups["shape"])
            };
        }

        var gridStrings = s.Split('\n').Select(s => s.Trim('\r'));
        var grids = new List<Problem>();

        foreach (var grid in gridStrings)
        {
            var gridMatch = GridRegex().Match(grid);

            if (gridMatch.Success)
            {
                grids.Add(new Problem
                {
                    Width = int.Parse(gridMatch.Groups["width"].Value),
                    Height = int.Parse(gridMatch.Groups["height"].Value),
                    ShapeCounts = [.. gridMatch.Groups["shapeCount"].Captures.Select(c => int.Parse(c.Value))]
                });
            }
            else
                throw new ArgumentOutOfRangeException(nameof(s));
        }

        return new ProblemList(grids);
    }

    private static List<(int y, int x)> TranslateCoords(Group group)
    {
        var height = group.Length;
        if (height == 0)
            return [];

        var width = group.Captures[0].Length;
        Debug.Assert(group.Captures.All(g => g.Length == width));

        var result = new List<(int y, int x)>();

        for (var i = 0; i < height; i++)
        {
            var line = group.Captures[i].Value;
            for (var j = 0; j < width; j++)
            {
                if (line[j] == '#')
                    result.Add((i, j));
            }
        }

        return result;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Instruction result) => throw new NotImplementedException();

    [GeneratedRegex("^(?<number>[0-9]*):(?:\r\n(?<shape>[#\\.]+))+$")]
    private static partial Regex ShapeRegex();

    [GeneratedRegex("^(?<width>[0-9]+)x(?<height>[0-9]+):(?: (?<shapeCount>[0-9]+))+$")]
    private static partial Regex GridRegex();
}
