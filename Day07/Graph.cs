using AdventOfCode2025.Shared.Graphs;

namespace AdventOfCode2025.Day07;

public record Node
{
    public required (int y, int x) Position { get; init; }
    public bool IsSplitter { get; set; }

    public override string ToString() => $"({Position.y}, {Position.x})";
}

public record Graph : Graph<Node, Edge<Node>>
{
    public long CountPaths(int height) => CountPaths(n => n.Position.y == height - 1);
}