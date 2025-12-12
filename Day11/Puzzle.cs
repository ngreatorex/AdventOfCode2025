using AdventOfCode2025.Shared;
using Serilog;

namespace AdventOfCode2025.Day11;

public class Puzzle : Puzzle<Puzzle, Device>
{
    public Dictionary<string, Device> Devices { get; init; } = [];

    public override void LogState(bool isInitial)
    {
        foreach (var device in Devices)
        {
            Log.Debug("{ID}: {Connections}",
                device.Key,
                string.Join(" ", device.Value.ConnectedDeviceIDs));
        }
    }

    public override async Task Run()
    {
        await base.Run();

        var nodes = Devices
            .Append(new KeyValuePair<string, Device>("out", new Device { ID = "out" }))
            .ToDictionary(kvp => kvp.Key, kvp => new Node { DeviceID = kvp.Value.ID });

        var start = nodes.Values.SingleOrDefault(n => n.DeviceID == "you") ?? nodes.Values.Single(n => n.DeviceID == "svr");

        var edges = Devices.SelectMany(kvp =>
        {
            return kvp.Value.ConnectedDeviceIDs.Select(d => new Edge
            {
                Source = nodes[kvp.Key],
                Target = nodes[d]
            });
        });

        var graph = new Graph()
        {
            Nodes = [.. nodes.Values],
            Edges = [.. edges],
            Start = start
        };


        Part1(nodes, graph);
        Part2(nodes, graph);
    }

    private void Part1(Dictionary<string, Node> nodes, Graph graph)
    {
        var endNode = nodes["out"];
        var pathCount = graph.CountPaths(n => n == endNode);
        Log.Information("Found {Count} paths from {Start} to node {Finish}", pathCount, graph.Start, endNode);
    }

    private void Part2(Dictionary<string, Node> nodes, Graph graph)
    {
        if (!nodes.TryGetValue("dac", out Node? dac))
            return;
        if (!nodes.TryGetValue("fft", out Node? fft))
            return;

        var startGraph = graph with { Start = nodes["svr"] };
        var @out = nodes["out"];

        var startToFftCount = startGraph.CountPaths(n => n == fft);
        var startToDacCount = startGraph.CountPaths(n => n == dac);

        var graphFromFft = graph with { Start = fft };
        var graphFromDac = graph with { Start = dac };

        var fftToDacCount = graphFromFft.CountPaths(n => n == dac);
        var dacToGoalCount = graphFromDac.CountPaths(n => n == @out);

        var dacToFftCount = graphFromDac.CountPaths(n => n == fft);
        var fftToGoalCount = graphFromFft.CountPaths(n => n == @out);

        var totalCount = checked(startToFftCount * fftToDacCount * dacToGoalCount + startToDacCount * dacToFftCount * fftToGoalCount);

        Log.Information("Found {Count} paths from {Start} to node {End} visiting {FFT} and {DAC}",
            totalCount, graph.Start, @out, fft, dac);
    }

    protected override Task ProcessInstruction(Device instruction)
    {
        Devices.Add(instruction.ID, instruction);

        return Task.CompletedTask;
    }
}
