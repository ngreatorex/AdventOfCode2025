using AdventOfCode2025.Shared;
using Serilog.Events;

namespace AdventOfCode2025.Day12;

partial class Program : Program<Program, Puzzle, Instruction>
{
    public Program()
    {
        MinimumLogLevel = LogEventLevel.Debug;
        LogOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] [{InputFile}] [{Problem}] {Message:lj}{NewLine}{Exception}";
    }

    public static new async Task Main(string[] args) => await Program<Program, Puzzle, Instruction>.Main(args);
}