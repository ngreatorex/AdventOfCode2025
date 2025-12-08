using Serilog;
using Serilog.Context;
using Serilog.Sinks.SystemConsole.Themes;

namespace AdventOfCode2025.Shared;

public abstract class Program<TSelf, TPuzzle, TInstruction>
    where TSelf : Program<TSelf, TPuzzle, TInstruction>, new()
    where TPuzzle : Puzzle<TPuzzle, TInstruction>, IPuzzle<TPuzzle, TInstruction>, new()
    where TInstruction : IParsable<TInstruction>
{
    protected virtual bool PartTwoRequiresRerun { get; } = false;

    protected virtual async Task Run()
    {
        ConfigureLogger();

        foreach (var file in InputFiles)
        {
            await RunFile(file);
        }
    }

    protected virtual void ConfigureLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{InputFile}] {Message:lj}{NewLine}{Exception}",
                theme: AnsiConsoleTheme.Literate)
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .CreateLogger();
    }

    protected virtual IEnumerable<string> InputFiles => Directory.EnumerateFiles("inputs", "*.txt").Reverse();

    protected async Task RunFile(string filename)
    {
        using var l = LogContext.PushProperty("InputFile", Path.GetFileName(filename));
        var puzzleInput = await TPuzzle.LoadAsync<TPuzzle>(filename, false);

        puzzleInput.LogState(true);
        Log.Information("Processing file {FileName}", filename);
        puzzleInput.Run();
        puzzleInput.LogState(false);

        if (PartTwoRequiresRerun)
        {
            var partTwoInput = await TPuzzle.LoadAsync<TPuzzle>(filename, true);

            partTwoInput.LogState(true);
            Log.Information("Processing file {FileName} for part two", filename);
            partTwoInput.Run();
            partTwoInput.LogState(false);
        }
    }

    public static async Task Main(string[] args)
    {
        await new TSelf().Run();
        await Log.CloseAndFlushAsync();
    }
}
