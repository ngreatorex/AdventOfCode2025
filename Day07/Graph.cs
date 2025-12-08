using System.Collections.Concurrent;

namespace AdventOfCode2025.Day07;

public record Node
{
    public required (int y, int x) Position { get; init; }
    public bool IsSplitter { get; set; }

    public override string ToString() => $"({Position.y}, {Position.x})";
}

public record Edge
{
    public required Node Source { get; init; }
    public required Node Target { get; init; }

    public override string ToString() => $"{Source} => {Target}";
}

public class Graph
{
    public HashSet<Node> Nodes { get; init; } = [];
    public List<Edge> Edges { get; init; } = [];

    public required Node Start { get; init; }

    public long CountPaths(int height)
    {
        var endNodes = Nodes.Where(n => n.Position.y == height - 1).ToList();
        var memo = new ConcurrentDictionary<Node, long>();
        return endNodes.AsParallel().Sum(n => CountPaths(n, []));
    }

    private long CountPaths(Node node, ConcurrentDictionary<Node, long> memo) => CountPaths(Start, node, memo);

    private long CountPaths(Node currentNode, Node destination, ConcurrentDictionary<Node, long> memo)
    {
        if (currentNode == destination)
            return 1;

        if (memo.TryGetValue(currentNode, out long value))
            return value;

        long count = 0;
        foreach (var edge in Edges.Where(e => e.Source == currentNode))
        {
            count += CountPaths(edge.Target, destination, memo);
        }

        return memo[currentNode] = count;
    }
}