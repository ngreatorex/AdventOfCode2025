using Serilog;
using System.Text;

namespace AdventOfCode2025.Day12;

public static class PolyominoEnumerableExtensions
{
    public static IEnumerable<string> ToStrings(this IEnumerable<Polyomino> polyominos)
    {
        var chars = polyominos.Select((p, i) => (p, i)).ToDictionary(p => p.p, p => (char)('A' + p.i));
        var maxX = polyominos.SelectMany(p => p.Coords).Max(c => c.x);
        var maxY = polyominos.SelectMany(p => p.Coords).Max(c => c.y);

        var grid = new char[maxY + 1, maxX + 1];
        for (var i = 0; i < grid.GetLength(0); i++)
            for (var j = 0; j < grid.GetLength(1); j++)
                grid[i, j] = '.';

        foreach (var (p, c) in polyominos.SelectMany(p => p.Coords.Select(c => (p, c))))
            grid[c.y, c.x] = chars[p];

        for (var i = 0; i < grid.GetLength(0); i++)
        {
            var sb = new StringBuilder(grid.GetLength(1));
            for (var j = 0; j < grid.GetLength(1); j++)
                sb.Append(grid[i, j]);

            yield return sb.ToString();
        }
    }
}

public record Polyomino : Instruction
{
    private static Func<(int y, int x), (int y, int x)>[] Rotations =>
    [
        static p => (p.y, p.x),   //   0 degrees
        static p => (p.x, -p.y),  //  90 degrees
        static p => (-p.y, -p.x), // 180 degrees
        static p => (-p.x, p.y)   // 270 degrees
    ];

    private static Func<(int y, int x), (int y, int x)> ReflectCoord => static p => (p.y, -p.x);

    public int Number { get; init; }
    public required List<(int y, int x)> Coords { get; init; }

    public int Width => Coords.Max(c => c.x) - Coords.Min(c => c.x) + 1;
    public int Height => Coords.Max(c => c.y) - Coords.Min(c => c.y) + 1;
    public int Area => checked(Width * Height);

    public Polyomino Translate(int dy, int dx)
    {
        return this with
        {
            Coords = [.. Coords.Select(c => (c.y + dy, c.x + dx))]
        };
    }

    public Polyomino Rotate(int n)
    {
        var index = n % 4;
        return this with
        {
            Coords = [.. Coords.Select(Rotations[index])]
        };
    }

    public Polyomino Reflect()
    {
        return this with
        {
            Coords = [.. Coords.Select(ReflectCoord)]
        };
    }

    public Polyomino Normalize()
    {
        var minX = Coords.Min(c => c.x);
        var minY = Coords.Min(c => c.y);

        return Translate(-minY, -minX);
    }

    public bool Overlaps(Polyomino other)
    {
        var pairs = from mine in Coords
                    from theirs in other.Coords
                    where (mine.y == theirs.y && mine.x == theirs.x)
                    select true;

        return pairs.Any();
    }

    public IEnumerable<Polyomino> GeneratePossiblePlacements(int height, int width)
    {
        var uniqueConfigurations = new HashSet<Polyomino>();

        for (var rotation = 0; rotation < 4; rotation++)
        {
            for (var reflection = false; !reflection; reflection = !reflection)
            {
                var config = Rotate(rotation);
                if (reflection)
                    config = config.Reflect();
                config = config.Normalize();

                if (!uniqueConfigurations.Add(config))
                    continue;

                for (var dy = 0; dy < width; dy++)
                {
                    for (var dx = 0; dx < width; dx++)
                    {
                        var position = config.Translate(dy, dx);

                        if (position.Coords.All(c => c.y < height && c.x < width))
                            yield return position;
                    }
                }
            }
        }
    }

    public void LogState(bool isInitial)
    {
        var array = new Polyomino[1] { this };
        Log.Information("Shape {ID}:", Number);
        foreach (var line in array.ToStrings())
            Log.Information(" " + line);
    }

}
