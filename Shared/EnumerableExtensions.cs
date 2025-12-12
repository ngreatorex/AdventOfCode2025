namespace AdventOfCode2025.Shared;

public static class EnumerableExtensions
{
    public static IEnumerable<(T a, T b)> GetOrderedPairs<T>(this IList<T> list) => list.SelectMany((t, i) => list.Where((_, j) => j > i), (t1, t2) => (t1, t2));
}
