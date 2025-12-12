namespace AdventOfCode2025.Shared.Maths;

using static Products;
public record Angle(Point D, int Turns)
{
    private static bool Half(Point p)
    {
        return p.Y > 0 || (p.Y == 0 && p.X < 0);
    }

    private static bool TupleLessThan((int a, bool b, long c) a, (int a, bool b, long c) b)
    {
        if (a.a < b.a)
            return true;
        if (!a.b && b.b)
            return true;
        if (a.c < b.c)
            return true;

        return false;
    }

    private static bool TupleGreaterThan((int a, bool b, long c) a, (int a, bool b, long c) b)
    {
        if (a.a > b.a)
            return true;
        if (a.b && !b.b)
            return true;
        if (a.c > b.c)
            return true;

        return false;
    }

    internal Angle MoveTo(Angle a, Point newD)
    {
        var b = new Angle(newD, a.Turns);

        if (b.Turn180() < b)
            b = b with { Turns = Turns - 1 };
        if (b.Turn180() < a)
            b = b with { Turns = Turns - 1 };

        return b;
    }

    internal Angle Turn180() => this with { D = D * -1, Turns = Turns + (Half(D) ? 1 : 0) };
    internal Angle Turn360() => this with { Turns = Turns + 1 };

    public static bool operator <(Angle a, Angle b) => TupleLessThan((a.Turns, Half(a.D), 0), (b.Turns, Half(a.D), Cross(a.D, b.D)));
    public static bool operator >(Angle a, Angle b) => TupleGreaterThan((a.Turns, Half(a.D), 0), (b.Turns, Half(a.D), Cross(a.D, b.D)));
}
