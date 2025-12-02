using Serilog;

namespace AdventOfCode2025.Shared;

public abstract class Program<TSelf, TPuzzle, TInstruction>
    where TSelf : Program<TSelf, TPuzzle, TInstruction>, new()
    where TPuzzle : Puzzle<TPuzzle, TInstruction>, IPuzzle<TPuzzle, TInstruction>, new()
    where TInstruction : IParsable<TInstruction>, new()
{
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
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .CreateLogger();
    }

    protected virtual IEnumerable<string> InputFiles => Directory.EnumerateFiles("inputs", "*.txt");

    protected async Task RunFile(string filename)
    {
        var puzzleInput = await TPuzzle.LoadAsync<TPuzzle>(filename);

        Log.Information("Processing file {FileName}", filename);
        puzzleInput.Run();
        puzzleInput.PrintResult();
    }

    public static async Task Main(string[] args)
    {
        await new TSelf().Run();
    }
}
