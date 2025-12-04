using Serilog;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AdventOfCode2025.Day04;

public class Grid : IParsable<Grid>
{
    public Cell[,] Cells { get; init; } = new Cell[0, 0];

    public int Height => Cells.GetLength(0);
    public int Width => Cells.GetLength(1);

    public int AccessibleRollCount
    {
        get
        {
            var total = 0;

            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    if (CellContainsRoll(i, j) && CellIsAccessible(i, j))
                        total++;
                }
            }

            return total;
        }
    }

    public int RemoveAccessibleRolls()
    {
        var removedRolls = 0;

        for (var i = 0; i < Height; i++)
        {
            for (var j = 0; j < Width; j++)
            {
                if (CellContainsRoll(i, j) && CellIsAccessible(i, j))
                {
                    removedRolls++;
                    Cells[i, j] = Cells[i, j] with { ContainsRoll = false };
                }
            }
        }

        return removedRolls;
    }

    public void LogGrid()
    {
        for (var i = 0; i < Height; i++)
        {
            var sb = new StringBuilder(Width);
            for (var j = 0; j < Width; j++)
            {
                if (CellContainsRoll(i, j))
                {
                    if (CellIsAccessible(i, j))
                    {
                        sb.Append('x');
                    }
                    else
                    {
                        sb.Append('@');
                    }
                }
                else
                {
                    sb.Append('.');
                }
            }

            Log.Information("Row {Row}: {Cells}", i + 1, sb.ToString());
        }
    }

    private bool CellContainsRoll(int y, int x) => Cells[y, x].ContainsRoll;
    private bool CellIsAccessible(int y, int x) => GetNeighbours(y, x).Sum(n => Cells[n.y, n.x].ContainsRoll ? 1 : 0) < 4;

    private IEnumerable<(int y, int x)> GetNeighbours(int y, int x)
    {
        if (y > 0) yield return (y - 1, x); // North
        if (x < Width - 1 && y > 0) yield return (y - 1, x + 1); // North East
        if (x < Width - 1) yield return (y, x + 1); // East
        if (x < Width - 1 && y < Height - 1) yield return (y + 1, x + 1); // South East
        if (y < Height - 1) yield return (y + 1, x); // South
        if (y < Height - 1 && x > 0) yield return (y + 1, x - 1); // South West
        if (x > 0) yield return (y, x - 1); // West
        if (y > 0 && x > 0) yield return (y - 1, x - 1); // North West
    }

    public static Grid Parse(string s, IFormatProvider? provider)
    {
        var lines = s.Split('\n').Select(s => s.Trim('\r')).ToList();
        var height = lines.Count;

        if (height == 0)
            throw new ArgumentOutOfRangeException(nameof(s), "Empty grid provided");

        var width = lines[0].Length;
        Debug.Assert(lines.All(l => l.Length == width));

        var grid = new Grid()
        {
            Cells = new Cell[height, width]
        };

        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                var c = lines[i][j];

                grid.Cells[i, j] = new Cell
                {
                    ContainsRoll = c == '@'
                };
            }
        }

        return grid;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Grid result) => throw new NotImplementedException();


}
public record Cell
{
    public bool ContainsRoll { get; init; }

}
