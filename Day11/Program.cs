using AdventOfCode2025.Shared;
using Serilog.Events;

namespace AdventOfCode2025.Day11;

partial class Program : Program<Program, Puzzle, Device>
{
    public Program()
    {
        MinimumLogLevel = LogEventLevel.Debug;
    }

    public static new async Task Main(string[] args) => await Program<Program, Puzzle, Device>.Main(args);
}