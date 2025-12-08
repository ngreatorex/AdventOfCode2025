using AdventOfCode2025.Shared;
using Serilog;

namespace AdventOfCode2025.Day04;

public class Puzzle : Puzzle<Puzzle, Grid>
{
    protected override IEnumerable<string> Split(string input, bool _) => [input];
    protected Grid? Input { get; set; }

    public override void LogState(bool isInitial)
    {
        if (Input == null)
            return;

        Input.LogGrid();

        Log.Information("Grid initially contains {Count} rolls that are accessible by forklift", Input.AccessibleRollCount);

        var totalRemoved = 0;
        while (Input.AccessibleRollCount > 0)
        {
            var removedCount = Input.RemoveAccessibleRolls();
            Log.Information("Removed {Count} rolls, now {AccessibleCount} are accessible", removedCount, Input.AccessibleRollCount);
            Input.LogGrid();

            totalRemoved += removedCount;
        }

        Log.Information("Removed {Total} rolls in total", totalRemoved);
    }

    protected override void ProcessInstruction(Grid instruction)
    {
        Input = instruction;
    }
}
