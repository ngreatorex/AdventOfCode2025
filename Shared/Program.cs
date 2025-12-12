using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace AdventOfCode2025.Shared;

public abstract class Program<TSelf, TPuzzle, TInstruction>
    where TSelf : Program<TSelf, TPuzzle, TInstruction>, new()
    where TPuzzle : Puzzle<TPuzzle, TInstruction>, IPuzzle<TPuzzle, TInstruction>, new()
    where TInstruction : IParsable<TInstruction>
{
    protected virtual bool PartTwoRequiresRerun { get; } = false;
    protected LogEventLevel MinimumLogLevel { get; set; } = LogEventLevel.Information;
    protected string LogOutputTemplate { get; set; } = "[{Timestamp:HH:mm:ss} {Level:u3}] [{InputFile}] {Message:lj}{NewLine}{Exception}";

    protected virtual async Task Run()
    {
        ConfigureLogger();

        foreach (var file in InputFiles)
        {
            await RunFile(file);
        }
    }

    protected virtual void ConfigureLogger(params Action<LoggerConfiguration>[] actions)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: LogOutputTemplate, theme: AnsiConsoleTheme.Literate)
            .Enrich.FromLogContext()
            .MinimumLevel.Is(MinimumLogLevel)
            .CreateLogger();
    }

    protected virtual IEnumerable<string> InputFiles => Directory.EnumerateFiles("inputs", "*.txt").Reverse();

    protected async Task RunFile(string filename)
    {
        using var l = LogContext.PushProperty("InputFile", Path.GetFileName(filename));
        var puzzleInput = await TPuzzle.LoadAsync<TPuzzle>(filename, false);

        Log.Information("Processing file {FileName}", filename);
        await puzzleInput.Run();
        puzzleInput.LogState(false);

        if (PartTwoRequiresRerun)
        {
            var partTwoInput = await TPuzzle.LoadAsync<TPuzzle>(filename, true);

            Log.Information("Processing file {FileName} for part two", filename);
            await partTwoInput.Run();
            partTwoInput.LogState(false);
        }
    }

    public static async Task Main(string[] args)
    {
        await new TSelf().Run();
        await Log.CloseAndFlushAsync();
    }
}
