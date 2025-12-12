namespace AdventOfCode2025.Shared.Maths;

public static class Products
{
    public static long Dot(Point a, Point b)
    {
        return checked(a.X * b.X + a.Y * b.Y);
    }

    public static long Cross(Point a, Point b)
    {
        return checked(a.X * b.Y - a.Y * b.X);
    }
}
