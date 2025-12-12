using AdventOfCode2025.Shared;
using Serilog;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace AdventOfCode2025.Day08;

public class Puzzle : Puzzle<Puzzle, JunctionBox>
{
    private int NumberOfExtensionLeads => JunctionBoxes.Count > 20 ? 1000 : 10;

    private List<JunctionBox> JunctionBoxes { get; init; } = [];
    private List<(JunctionBox a, JunctionBox b)> Connections { get; init; } = [];

    private SortedDictionary<double, (JunctionBox a, JunctionBox b)> SortedEuclideanDistances { get; } = [];
    private ConcurrentDictionary<JunctionBox, List<(JunctionBox a, JunctionBox b)>> ConnectionsFromNode { get; } = [];

    private void PreCalculateEuclideanDistances()
    {
        var pairs = JunctionBoxes.GetOrderedPairs().ToList();
        foreach (var pair in pairs)
        {
            SortedEuclideanDistances.Add(EuclideanDistance(pair.a, pair.b), pair);
        }
    }

    public override void LogState(bool isInitial)
    {
        Log.Information("Current state: {Count} junction boxes, {Connections} connections", JunctionBoxes.Count, Connections.Count);
    }

    protected override Task ProcessInstruction(JunctionBox instruction)
    {
        JunctionBoxes.Add(instruction);

        return Task.CompletedTask;
    }

    public override async Task Run()
    {
        await base.Run();

        PreCalculateEuclideanDistances();

        for (int i = 0; i < NumberOfExtensionLeads; i++)
            Step();

        Debug.Assert(!Connections.Any(p1 => Connections.Contains((p1.b, p1.a))), "No loops");

        var subsets = GenerateConnectedSubsets();
        subsets.Sort((a, b) => b.Count - a.Count);
        Log.Debug("After {ConnectionCount} connections, found {Count} connected subsets: {Subsets}", Connections.Count, subsets.Count, subsets);

        var largestThree = subsets.Take(3).ToList();
        int a = largestThree[0].Count, b = largestThree[1].Count, c = largestThree[2].Count;
        var d = a * b * c;
        Log.Information("Largest 3 subsets have size {A}, {B} and {C} (product {D})", a, b, c, d);

        (JunctionBox a, JunctionBox b)? lastPair = null;

        while (a < JunctionBoxes.Count || lastPair == null)
        {
            lastPair = Step();

            subsets = GenerateConnectedSubsets();
            a = subsets.Max(s => s.Count);

            Log.Debug("After {ConnectionCount} connections, found {Count} connected subsets. Largest subset is {A} boxes.", Connections.Count, subsets.Count, a);
        }

        Log.Information("Last connection was between {A} and {B}. Product of their X coordinates is {XProduct}", lastPair.Value.a, lastPair.Value.b, (long)lastPair.Value.a.X * lastPair.Value.b.X);
    }

    private List<HashSet<JunctionBox>> GenerateConnectedSubsets()
    {
        var visited = new HashSet<JunctionBox>();
        var subsets = new List<HashSet<JunctionBox>>();

        foreach (var j in JunctionBoxes)
        {
            if (visited.Add(j))
            {
                var subset = DFS(j);
                subsets.Add(subset);
                visited.UnionWith(subset);
            }
        }

        return subsets;
    }

    private HashSet<JunctionBox> DFS(JunctionBox start)
    {
        var visited = new HashSet<JunctionBox>();
        var s = new Stack<JunctionBox>();
        s.Push(start);

        while (s.TryPop(out var j))
        {
            if (visited.Add(j))
            {
                foreach (var (a, b) in ConnectionsFromNode.GetOrAdd(j, []))
                {
                    if (a != j)
                        s.Push(a);
                    else
                        s.Push(b);
                }
            }
        }

        return visited;
    }

    private (JunctionBox a, JunctionBox b) Step()
    {
        var (dist, lowestDistancePair) = SortedEuclideanDistances.First();

        Log.Debug("Connecting {A} and {B} (distance {D:F2})", lowestDistancePair.a, lowestDistancePair.b, dist);

        Connections.Add(lowestDistancePair);
        ConnectionsFromNode.GetOrAdd(lowestDistancePair.a, []).Add(lowestDistancePair);
        ConnectionsFromNode.GetOrAdd(lowestDistancePair.b, []).Add(lowestDistancePair);

        SortedEuclideanDistances.Remove(dist);

        return lowestDistancePair;
    }

    private static double EuclideanDistance(JunctionBox a, JunctionBox b) => Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2) + Math.Pow(b.Z - a.Z, 2));
}
