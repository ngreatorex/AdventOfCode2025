using AdventOfCode2025.Shared;
using AdventOfCode2025.Shared.Maths;
using Serilog;
using System.Drawing;
using System.Text;
using Point = AdventOfCode2025.Shared.Maths.Point;

namespace AdventOfCode2025.Day09;

public class Puzzle : Puzzle<Puzzle, RedTile>
{
    public List<RedTile> RedTiles { get; init; } = [];

    public int Height => RedTiles.Count > 0 ? RedTiles.Max(c => c.Y) + 2 : 0;
    public int Width => RedTiles.Count > 0 ? RedTiles.Max(c => c.X) + 2 : 0;

    public override async Task Run()
    {
        await base.Run();

        Part1();
        var square = Part2();
        LogGridWithSquare(square);
    }

    private void Part1()
    {
        var pairs = RedTiles.GetOrderedPairs();

        (RedTile a, RedTile b)? maxPair = null;
        long maxArea = 0;

        foreach (var pair in pairs)
        {
            var size = (Math.Abs(pair.b.Y - pair.a.Y) + 1, Math.Abs(pair.b.X - pair.a.X) + 1);
            var area = (long)size.Item1 * size.Item2;

            Log.Verbose("Rectangle between {A} and {B} has area {Area}", pair.a, pair.b, area);

            if (area > maxArea)
            {
                maxArea = area;
                maxPair = pair;
            }
        }

        if (maxPair == null)
            throw new InvalidOperationException("Unable to find max pair");

        Log.Information("Max area of rectangle is {MaxArea} (corners {A} and {B})", maxArea, maxPair.Value.a, maxPair.Value.b);
    }

    private (Point a, Point b) Part2()
    {
        var polygon = new Polygon([.. RedTiles.Select(r => new Point(r.X, r.Y))]);
        var polygonPointCount = polygon.Points.Count;
        List<LineSegment> polygonEdges = new List<LineSegment>(polygonPointCount);

        for (var j = 0; j < polygonPointCount; j++)
        {
            polygonEdges.Add(new LineSegment(polygon.Points[j], polygon.Points[(j + 1) % polygonPointCount]));
        }

        (RedTile a, RedTile b)? max = null;
        long maxArea = 0;
        var lockObj = new object();

        foreach (var pair in RedTiles.GetOrderedPairs())
        {
            var minX = Math.Min(pair.a.X, pair.b.X);
            var minY = Math.Min(pair.a.Y, pair.b.Y);
            var maxX = Math.Max(pair.a.X, pair.b.X);
            var maxY = Math.Max(pair.a.Y, pair.b.Y);

            List<Point> corners =
            [
                new Point(minX, minY),
                new Point(minX, maxY),
                new Point(maxX, maxY),
                new Point(maxX, minY)
            ];

            if (corners.All(p => p.IsIn(polygon)))
            {
                var area = (Math.Abs((long)pair.b.Y - pair.a.Y) + 1) * (Math.Abs(pair.b.X - pair.a.X) + 1);

                if (area < maxArea)
                    continue;

                bool isValid = true;

                for (var i = 0; i < corners.Count; i++)
                {
                    var edgeOfSquare = new LineSegment(corners[i], corners[(i + 1) % corners.Count]);
                    var intersections = polygonEdges.Select(pe => (pe, pe.ProperIntersects(edgeOfSquare)));
                    if (intersections.Any(i => i.Item2))
                    {
                        Log.Verbose("Rectangle between {A} and {B}: Square edge {Edge1} intersects polygon edge {Edge2}", pair.a, pair.b, edgeOfSquare, intersections.First(i => i.Item2).pe);
                        isValid = false;
                        break;
                    }
                }

                if (!isValid)
                    continue;

                Log.Verbose("Rectangle between {A} and {B} is inside the polygon and has area {Area}", pair.a, pair.b, area);

                if (area > maxArea)
                {
                    maxArea = area;
                    max = pair;
                }
            }
            else
            {
                Log.Verbose("Rectangle between {A} and {B} does not have all 4 corners in the polygon", pair.a, pair.b);
            }
        }

        if (max == null)
            throw new InvalidOperationException("Unable to find max pair");

        Log.Information("Max area of rectangle inside polygon is {MaxSize} (corners {A} and {B})", maxArea, max.Value.a, max.Value.b);
        return (max.Value.a.Point, max.Value.b.Point);
    }

    public override void LogState(bool isInitial)
    {
        LogGridWithSquare(null);
    }

    private const int scaleFactor = 10;

    internal void DrawBitmap(string fileName, (Point a, Point b)? square)
    {
        using var bitmap = new Bitmap(Width / scaleFactor, Height / scaleFactor);
        using var g = Graphics.FromImage(bitmap);
        using var redPen = new Pen(Color.Red, 1);
        using var greenPen = new Pen(Color.Green, 1);

        var points = RedTiles.Append(RedTiles[0]).Select(r => r.Point).ToList();

        for (var i = 0; i < points.Count; i++)
        {
            g.DrawLine(redPen, points[i].X / scaleFactor, points[i].Y / scaleFactor,
                points[(i + 1) % points.Count].X / scaleFactor,
                points[(i + 1) % points.Count].Y / scaleFactor);
        }

        if (square != null)
        {
            var minX = Math.Min(square.Value.a.X, square.Value.b.X);
            var minY = Math.Min(square.Value.a.Y, square.Value.b.Y);
            var width = Math.Abs(square.Value.b.X - square.Value.a.X);
            var height = Math.Abs(square.Value.b.Y - square.Value.a.Y);

            g.DrawRectangle(greenPen, minX / scaleFactor, minY / scaleFactor, width / scaleFactor, height / scaleFactor);
        }

        bitmap.Save(fileName);
    }

    internal void LogGridWithSquare((Point a, Point b)? square = default)
    {
        if (Height < 6 || Width < 6 || Height > 1000 || Width > 1000)
        {
            Log.Information("Grid with {Count} red tiles", RedTiles.Count);

            if (Width == 0 || Height == 0)
                return;

            DrawBitmap(square == null ? "output1.bmp" : "output2.bmp", square);

            return;
        }

        var cells = new char[Height, Width];
        var polygon = new Polygon([.. RedTiles.Append(RedTiles[0]).Select(r => new Point(r.X, r.Y))]);

        for (var i = 0; i < Height; i++)
            for (var j = 0; j < Width; j++)
                cells[i, j] = '.';

        foreach (var tile in RedTiles)
        {
            cells[tile.Y, tile.X] = 'R';
        }

        for (var i = 0; i < Height; i++)
        {
            for (var j = 0; j < Width; j++)
            {
                if (cells[i, j] != 'R' && (new Point(j, i)).IsIn(polygon))
                    cells[i, j] = 'G';
            }
        }

        if (square != null)
        {
            var minY = Math.Min(square.Value.a.Y, square.Value.b.Y);
            var maxY = Math.Max(square.Value.a.Y, square.Value.b.Y);
            var minX = Math.Min(square.Value.a.X, square.Value.b.X);
            var maxX = Math.Max(square.Value.a.X, square.Value.b.X);

            for (var i = minY; i <= maxY; i++)
            {
                for (var j = minX; j <= maxX; j++)
                {
                    cells[i, j] = '*';
                }
            }
        }

        Log.Information("Current grid:");
        for (var i = 0; i < Height; i++)
        {
            var sb = new StringBuilder(Width);
            for (var j = 0; j < Width; j++)
            {
                sb.Append(cells[i, j]);
            }
            Log.Information(sb.ToString());
        }
    }

    protected override Task ProcessInstruction(RedTile instruction)
    {
        RedTiles.Add(instruction);

        return Task.CompletedTask;
    }
}
