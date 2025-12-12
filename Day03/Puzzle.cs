using AdventOfCode2025.Shared;
using Serilog;

namespace AdventOfCode2025.Day03;

public class Puzzle : Puzzle<Puzzle, BatteryBank>
{
    public List<BatteryBank> BatteryBanks { get; init; } = [];

    public long GetTotalJoltage(int batteryCount) => BatteryBanks.Sum(b => b.GetMaxJoltage(batteryCount));

    public override void LogState(bool isInitial)
    {
        Log.Information("Total output joltage from two batteries: {TotalJoltage}", GetTotalJoltage(2));
        Log.Information("Total output joltage from twelve batteries: {TotalJoltage}", GetTotalJoltage(12));
    }

    protected override Task ProcessInstruction(BatteryBank instruction)
    {
        BatteryBanks.Add(instruction);

        return Task.CompletedTask;
    }
}
