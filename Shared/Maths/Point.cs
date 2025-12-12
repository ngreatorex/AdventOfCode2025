using Serilog;

namespace AdventOfCode2025.Shared.Maths;

public record Point(long X, long Y)
{
    public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
    public static Point operator *(Point a, int b) => new(a.X * b, a.Y * b);

    public long IsLeftOf(LineSegment l)
    {
        var calc = (l.B.X - l.A.X) * (Y - l.A.Y) - (X - l.A.X) * (l.B.Y - l.A.Y);

        Log.Verbose("Point ({X},{Y}) is {direction} line from ({X1},{Y1}) to ({X2},{Y2})",
            X, Y, calc > 0 ? "left of" : calc == 0 ? "on" : "right of",
            l.A.X, l.A.Y, l.B.X, l.B.Y);

        return calc;
    }

    public bool IsOnHorizontalOrVerticalLineSegment(LineSegment l)
    {
        if (l.A.Y == l.B.Y && Y == l.A.Y)
        {
            return (l.A.X <= X && l.B.X >= X)
                || (l.B.X <= X && l.A.X >= X);
        }
        else if (l.A.X == l.B.X && X == l.A.X)
        {
            return (l.A.Y <= Y && l.B.Y >= Y)
                || (l.B.Y <= Y && l.A.Y >= Y);
        }

        return false;
    }

    public bool IsIn(Polygon p) => p.WindingNumber(this) != 0;

    public override string ToString() => $"({X},{Y})";
}
