using AdventOfCode2025.Shared.Maths;

namespace AdventOfCode2025.Day09;

public static class Test
{
    public static void Main(string[] args)
    {
        var segmentOne = new LineSegment(new Point(5467, 67420), new Point(94651, 67420));
        var segmentTwo = new LineSegment(new Point(74943, 90663), new Point(74943, 91665));

        var intersects = segmentOne.Intersects(segmentTwo);
        Console.WriteLine($"Do they intersect? {intersects}");
    }
}