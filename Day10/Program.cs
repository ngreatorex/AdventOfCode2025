using AdventOfCode2025.Shared;

namespace AdventOfCode2025.Day10;

partial class Program : Program<Program, Puzzle, Machine>
{
    public Program()
    {
        MinimumLogLevel = Serilog.Events.LogEventLevel.Information;
    }
    public static new async Task Main(string[] args) => await Program<Program, Puzzle, Machine>.Main(args);
}