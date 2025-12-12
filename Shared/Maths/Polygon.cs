namespace AdventOfCode2025.Shared.Maths;

public record Polygon(List<Point> Points)
{
    internal int WindingNumber(Point P)
    {
        int wn = 0;

        var V = Points.Append(Points[0]).ToArray();

        for (var i = 0; i < Points.Count; i++)
        {
            if (V[i].Y == P.Y && P.IsOnHorizontalOrVerticalLineSegment(new LineSegment(V[i], V[i + 1])))
            {
                return 1;
            }
            else if (V[i].Y <= P.Y)
            {
                if (V[i + 1].Y > P.Y)
                {
                    if (P.IsLeftOf(new LineSegment(V[i], V[i + 1])) >= 0)
                    {
                        ++wn;
                    }
                }
            }
            else
            {
                if (V[i + 1].Y <= P.Y)
                {
                    if (P.IsLeftOf(new LineSegment(V[i], V[i + 1])) < 0)
                    {
                        --wn;
                    }
                }
            }
        }

        return wn;
    }
}