using AdventOfCode2025.Shared;
using Serilog;

namespace AdventOfCode2025.Day10;

public class Puzzle : Puzzle<Puzzle, Machine>
{
    public List<Machine> Machines { get; init; } = [];

    public override void LogState(bool isInitial)
    {
        foreach (var machine in Machines)
            machine.LogState(isInitial);
    }

    public override async Task Run()
    {
        await base.Run();

        Part1();
        Part2();
    }

    private void Part2()
    {
        var results = new List<int>();

        foreach (var machine in Machines)
        {
            var result = machine.FindJoltagePathByLinearAlgebra();
            Log.Debug("Found path to goal with {Count} buttons", result);
            results.Add(result);
        }

        Log.Information("Total button presses for joltages on all machines is {Total}", results.Sum());
    }

    private void NaivePart2()
    {
        var results = new List<int>();

        foreach (var machine in Machines)
        {
            var path = machine.FindJoltagePathByBFS().ToList();
            Log.Information("Found path to goal with {Count} buttons", path.Count);
            Log.Debug("Path: {Path}", string.Join(' ', path.Select(b => b.ToString())));
            results.Add(path.Count);
        }

        Log.Information("Total button presses for joltages on all machines is {Total}", results.Sum());
    }

    private void Part1()
    {
        var results = new List<int>();

        foreach (var machine in Machines)
        {
            var path = machine.FindPathToGoal().ToList();
            Log.Debug("Found path to goal with {Count} buttons", path.Count);
            Log.Debug("Path: {Path}", string.Join(' ', path.Select(b => b.ToString())));
            results.Add(path.Count);
        }

        Log.Information("Total button presses for indicators on all machines is {Total}", results.Sum());
    }

    protected override Task ProcessInstruction(Machine instruction)
    {
        Machines.Add(instruction);

        return Task.CompletedTask;
    }
}
