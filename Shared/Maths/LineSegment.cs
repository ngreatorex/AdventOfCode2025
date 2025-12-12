namespace AdventOfCode2025.Shared.Maths;

using static Products;

public record LineSegment(Point A, Point B)
{
    private static long Orient(Point a, Point b, Point c)
    {
        return Products.Cross(b - a, c - a);
    }

    private static bool InDisk(Point a, Point b, Point p)
    {
        return Dot(a - p, b - p) <= 0;
    }

    private static bool PointOnSegment(Point a, Point b, Point p)
    {
        return Orient(a, b, p) == 0 && InDisk(a, b, p);
    }

    public bool Intersects(LineSegment b)
    {
        return ProperIntersects(b)
            || PointOnSegment(b.A, b.B, A)
            || PointOnSegment(b.A, b.B, B)
            || PointOnSegment(A, B, b.A)
            || PointOnSegment(A, B, b.B);
    }

    public bool ProperIntersects(LineSegment other)
    {
        Point a = A, b = B, c = other.A, d = other.B;

        long oa = Orient(c, d, a),
        ob = Orient(c, d, b),
        oc = Orient(a, b, c),
        od = Orient(a, b, d);

        var result = checked(((decimal)oa * ob) < 0 && ((decimal)oc * od) < 0);
        return result;
    }

    public override string ToString() => $"{A} -> {B}";
}
