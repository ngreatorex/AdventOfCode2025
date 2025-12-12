using Serilog;

namespace AdventOfCode2025.Shared;

public abstract class Puzzle<TSelf, TInstruction> : IPuzzle<TSelf, TInstruction>
    where TInstruction : IParsable<TInstruction>
    where TSelf : Puzzle<TSelf, TInstruction>, new()
{

    public List<TInstruction> Instructions { get; init; } = [];

    public virtual async Task Run()
    {
        foreach (var instruction in Instructions)
        {
            await ProcessInstruction(instruction);
        }
    }

    public abstract void LogState(bool isInitial);

    protected virtual IEnumerable<string> Split(string input, bool isPartTwo) => input.Split('\n');

    protected abstract Task ProcessInstruction(TInstruction instruction);

    public static async Task<TSelf> LoadAsync<TInput>(string fileName, bool isPartTwo)
    {
        using var streamReader = new StreamReader(fileName);
        var result = new TSelf();

        Log.Debug("Reading {FileName}", fileName);
        var inputString = await streamReader.ReadToEndAsync();

        result.Instructions.AddRange(
            result.Split(inputString, isPartTwo)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim('\r'))
                .Select(s => TInstruction.Parse(s, null))
        );

        return result;
    }
}
