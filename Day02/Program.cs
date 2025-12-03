using AdventOfCode2025.Day02;
using AdventOfCode2025.Shared;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

partial class Program : Program<Program, Puzzle, PuzzleInstruction>
{
    protected override void ConfigureLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{InputFile}] [{ID}] [{ChunkSize}] {Message:lj}{NewLine}{Exception}",
                theme: AnsiConsoleTheme.Literate)
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .CreateLogger();
    }

    public static new async Task Main(string[] args) => await Program<Program, Puzzle, PuzzleInstruction>.Main(args);
}